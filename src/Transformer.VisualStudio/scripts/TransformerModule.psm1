function Switch-Environment {
	[CmdletBinding()]
    param(
        [Parameter(Position = 0, Mandatory = $true)] [string]$environment,
		[Parameter()] [string]$PasswordFile,
		[Parameter()] [string]$Password,
		[Parameter()] [string]$SubEnvironment = ""
    )
	Write-Host "Transform templates for each project-folder in the current solution..."

	Get-Project -All | Where-Object  { $_.Type -ne 'Web Site' } | % { Invoke-TransformerDirectoryTransform -Environment $environment -SubEnvironment $SubEnvironment -Directory (Split-Path -parent $_.Fullname) -PasswordFile $PasswordFile -Password $Password }
}

function Get-Environments {
	[CmdletBinding()]
	param()

	Get-TransformerEnvironmentDir | Get-ChildItem | % { $_.Basename }
}

#function New-EncryptionKey {
#	[CmdletBinding()]
#	param(
#		[Parameter] [string]$PasswordFile
#	)
#
#	Invoke-CreateEncryptionKey -PasswordFile $PasswordFile
#}

function Protect-Environments {
	[CmdletBinding()]
	param(
		[Parameter()] [string]$PasswordFile,
		[Parameter()] [string]$Password
	)

	Write-Host "Encrypting all variables..."

	Protect-TransformerEnvironments -Directory $pwd -PasswordFile $PasswordFile -Password $Password
}

Export-ModuleMember Get-Environments
Export-ModuleMember Switch-Environment
#Export-ModuleMember Encrypt-Environments
#Export-ModuleMember Create-EncryptionKey

Register-TabExpansion 'Switch-Environment' @{ 'environment' = { Get-TransformerEnvironmentDir | Get-ChildItem | % { $_.Basename } } }