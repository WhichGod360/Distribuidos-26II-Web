param(
    [string]$BackendRoot = $env:BIBLIOTECA_BACKEND_ROOT,
    [ValidateSet('Debug','Release')][string]$Configuration = 'Debug'
)
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
if (-not $BackendRoot) {
    $downloads = Split-Path (Split-Path $repo -Parent) -Parent
    $BackendRoot = Join-Path $downloads 'Distribuidos-26II-main\Distribuidos-26II-main\Distribuidos-2026II'
}
if (-not (Test-Path (Join-Path $BackendRoot 'capaNegocio\capaNegocio.vbproj'))) {
    throw 'Indique -BackendRoot o BIBLIOTECA_BACKEND_ROOT con la ruta de Distribuidos-2026II.'
}
& dotnet build (Join-Path $repo 'tools\BackendWeb\BackendWeb.csproj') "-p:BackendRoot=$BackendRoot" --configuration $Configuration --source https://api.nuget.org/v3/index.json --nologo --verbosity quiet
if ($LASTEXITCODE -ne 0) { throw 'No se pudo compilar el backend compartido para el consumidor web.' }

# Actualiza las DLL de la entrega únicamente al invocar este script explícitamente.
$salida = Join-Path $repo "artifacts\backend\$Configuration\net472"
$destino = Join-Path $repo 'Distribuidos2026-II-Web\Bibliotecas'
New-Item -ItemType Directory -Path $destino -Force | Out-Null
foreach ($nombre in 'capaDatos','capaNegocio','capaDAO','capaConexion','Microsoft.AspNetCore.Cryptography.Internal','Microsoft.AspNetCore.Cryptography.KeyDerivation','System.Data.SqlClient') {
    Copy-Item -LiteralPath (Join-Path $salida "$nombre.dll") -Destination (Join-Path $destino "$nombre.dll") -Force
}
