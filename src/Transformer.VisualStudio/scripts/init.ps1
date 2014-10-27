param($installPath, $toolsPath, $package)

Import-Module (Join-Path $toolsPath "Transformer.PowerShell.dll") -Prefix Transformer
Import-Module (Join-Path $toolsPath TransformerModule.psm1)

Write-Host "Transformer loaded"
Write-Host ""
Write-Host "Following commands are available"
Write-Host "	- Switch-Environment		Transform templates for an environment"
#Write-Host "	- New-EncriptionKey			Creates a new encryption key"
#Write-Host "	- Protect-Environments		Encrypts all variables marked with do-encryption"