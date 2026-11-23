$ErrorActionPreference = 'Stop'
$backend = Join-Path $PSScriptRoot 'backend'
$bibliotecas = Join-Path $PSScriptRoot 'Bibliotecas'
$capas = 'capaConexion', 'capaDatos', 'capaDAO', 'capaNegocio'

dotnet build (Join-Path $backend 'capaNegocio/capaNegocio.vbproj') -c Release -f net472 --nologo
if ($LASTEXITCODE -ne 0) { throw 'No se pudieron compilar las capas para la web.' }

foreach ($capa in $capas) {
    $dll = Join-Path $backend "$capa/bin/Release/net472/$capa.dll"
    Copy-Item -LiteralPath $dll -Destination $bibliotecas -Force
}

Get-ChildItem -LiteralPath $bibliotecas -Filter '*.dll' -File |
    Sort-Object Name |
    ForEach-Object { '{0}  {1}' -f (Get-FileHash -LiteralPath $_.FullName).Hash, $_.Name } |
    Set-Content -LiteralPath (Join-Path $bibliotecas 'SHA256SUMS.txt') -Encoding ascii
