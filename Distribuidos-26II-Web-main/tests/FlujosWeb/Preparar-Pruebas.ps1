param([string]$DirectorioPruebas)
$ErrorActionPreference = 'Stop'
$repo = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
if (-not $DirectorioPruebas) {
    $base = if (Test-Path 'D:\') { 'D:\' } else { $env:TEMP }
    $DirectorioPruebas = Join-Path $base ('LecturaeHttp-' + [guid]::NewGuid().ToString('N'))
}
if (Test-Path $DirectorioPruebas) { throw 'Usa una carpeta nueva y vacía para aislar esta prueba.' }
New-Item -ItemType Directory -Path $DirectorioPruebas | Out-Null
$proyecto = Join-Path $PSScriptRoot 'BackendPrueba\BackendPrueba.csproj'
& dotnet build $proyecto -c Release "-p:BaseIntermediateOutputPath=$DirectorioPruebas\obj\" -o "$DirectorioPruebas\backend" --nologo -v quiet
if ($LASTEXITCODE -ne 0) { throw 'No se pudo construir el doble de pruebas.' }
$web = Join-Path $repo 'Distribuidos2026-II-Web\appWEB Distribuidos'
if (-not (Test-Path "$web\bin\appWEB Distribuidos.dll")) { throw 'Compila la web real antes de preparar la prueba.' }
Copy-Item -LiteralPath $web -Destination "$DirectorioPruebas\sitio" -Recurse
Copy-Item -LiteralPath "$DirectorioPruebas\backend\capaNegocio.dll" -Destination "$DirectorioPruebas\sitio\bin\capaNegocio.dll" -Force
Write-Host 'La copia usa un backend EN MEMORIA. No abre SQL ni modifica Somee.'
Write-Host 'En una consola ejecute IIS Express con esta carpeta y puerto 51844:'
Write-Host "$DirectorioPruebas\sitio"
Write-Host 'En otra consola: node tests/FlujosWeb/ejecutar-http.mjs'
Write-Host 'Reinicie la copia antes de repetir: las pruebas cambian sus datos y contraseñas ficticias.'
