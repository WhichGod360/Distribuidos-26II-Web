$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$repo = Join-Path $root 'Distribuidos-26II-Web-main'
$solucion = Join-Path $repo 'Distribuidos2026-II-Web\Distribuidos2026-II-Web.slnx'
$temporal = Join-Path $root '.local\Compilacion'
$aspnet = Join-Path $root '.local\AspNet'
New-Item -ItemType Directory -Path $temporal,$aspnet -Force | Out-Null
$env:TEMP = $temporal
$env:TMP = $temporal
$archivoConexion = Join-Path $env:LOCALAPPDATA 'Biblioteca\connection-string.txt'
if (-not (Test-Path -LiteralPath $archivoConexion)) { throw 'No se encontró el archivo privado de conexión de Biblioteca.' }
$env:BIBLIOTECA_CONNECTION_FILE = $archivoConexion
$config = Join-Path $repo 'Distribuidos2026-II-Web\appWEB Distribuidos\Web.config'
[xml]$xml = Get-Content -LiteralPath $config
$xml.configuration.'system.web'.compilation.SetAttribute('tempDirectory', $aspnet)
$xml.Save($config)
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$visualStudio = & $vswhere -latest -products Microsoft.VisualStudio.Product.Community Microsoft.VisualStudio.Product.Professional Microsoft.VisualStudio.Product.Enterprise -property productPath
if (-not $visualStudio -or -not (Test-Path -LiteralPath $visualStudio)) { throw 'No se encontró Visual Studio.' }
# La nueva instancia hereda los temporales de esta carpeta sin cambiar ajustes globales de Windows.
Start-Process -FilePath $visualStudio -ArgumentList ('"' + $solucion + '"') -WindowStyle Normal

