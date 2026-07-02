param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$OutputDir = "artifacts/win-x64"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

$project = "src/AncientValueOverlay/AncientValueOverlay.csproj"
$zipPath = "artifacts/AncientValueOverlay-$Runtime.zip"

if (Test-Path $OutputDir) { Remove-Item $OutputDir -Recurse -Force }
if (Test-Path $zipPath) { Remove-Item $zipPath -Force }
New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null
New-Item -ItemType Directory -Force -Path "artifacts" | Out-Null

dotnet restore $project
dotnet publish $project `
    --configuration $Configuration `
    --runtime $Runtime `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -p:DebugType=None `
    -p:DebugSymbols=false `
    --output $OutputDir

Compress-Archive -Path "$OutputDir/*" -DestinationPath $zipPath -Force

Write-Host "Package created: $zipPath"
Write-Host "Run: $OutputDir/AncientValueOverlay.exe"
