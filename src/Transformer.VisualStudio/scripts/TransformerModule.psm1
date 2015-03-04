$package_folder = Get-ChildItem (Join-Path (split-path $dte.Solution.FullName) "packages") -Filter Transformer.VisualStudio* | select -ExpandProperty Fullname -last 1
$transformer_exe = Join-Path $package_folder "tools/transformer.exe"

function Switch-Environment {
	[CmdletBinding()]
    param(
        [Parameter(Position = 0, Mandatory = $true)] [string]$environment,
		[Parameter()] [string]$PasswordFile = "",
		[Parameter()] [string]$Password = "",
		[Parameter()] [strinag]$SubEnvironment = ""
    )
	Write-Host "Transform templates for all files in the solution folder"
	
	$args = @("transform", "--environment", $environment, "--path", (Split-Path -parent $dte.Solution.Fullname))

	if ([string]::IsNullOrEmpty($Password) -eq $false) {
		$args += "--password"
		$args += $Password
	}

	if ([string]::IsNullOrEmpty($SubEnvironment) -eq $false) {
		$args += "--sub-environment"
		$args += $SubEnvironment
	}

	if ([string]::IsNullOrEmpty($PasswordFile) -eq $false) {
		$args += "--password-file"
		$args += $PasswordFile
	}

	if ($PSBoundParameters['Verbose']) {
		$args += "--verbose"
	}

	& $script:transformer_exe $args

	# old version took all project dirs, so a project could also be outside of the solution path.
	#Get-Project -All | Where-Object  { $_.Type -ne 'Web Site' } | % { `
	#	& $transformer_exe --environment $environment --sub-environment $SubEnvironment --path (Split-Path -parent $_.Fullname) --password-file $PasswordFile --password $Password }
	#	Invoke-TransformerDirectoryTransform -Environment $environment -SubEnvironment $SubEnvironment -Directory (Split-Path -parent $_.Fullname) -PasswordFile $PasswordFile -Password $Password }
}

function Get-Environments {
	[CmdletBinding()]
	param()
	& $script:transformer_exe list
	#Get-TransformerEnvironmentDir | Get-ChildItem | % { $_.Basename }
}

function New-EncryptionKey {
[CmdletBinding()]
	param(
		[Parameter] [string]$PasswordFile
	)

	Invoke-CreateEncryptionKey -PasswordFile $PasswordFile
}


#PM> ls .\packages -Filter Transformer* | sort -Descending | select -First 1

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
# TODO integrate encryption stuff

Register-TabExpansion 'Switch-Environment' @{ 'environment' = { & $script:transformer_exe list } }