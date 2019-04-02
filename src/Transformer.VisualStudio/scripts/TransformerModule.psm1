$transformer_exe = Join-Path $PSScriptRoot "transformer.exe"

function Switch-Environment {
	[CmdletBinding()]
    param(
        [Parameter(Position = 0, Mandatory = $true)] [string]$environment,
		[Parameter()] [string]$PasswordFile = "",
		[Parameter()] [string]$Password = "",
		[Parameter(Position = 1)] [string]$SubEnvironment = ""
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

function New-EncryptionKey {
[CmdletBinding()]
	param(
		[Parameter()] [string]$PasswordFile
	)

	$args = @("create-passwordfile", "--password-file", (Join-Path $script:sln_dir "password.txt"))

	& $script:transformer_exe $args
}

function Edit-EncryptionKey {
[CmdletBinding()]
	param(
		[Parameter(Position = 1)] [string]$OldPassword,
		[Parameter(Position = 2)] [string]$NewPassword
	)

	$args = @("change-password", "--old-password", $OldPassword, "--new-password", $NewPassword, "--path", $script:sln_dir)

	& $script:transformer_exe $args
}

function Protect-Environments {
	[CmdletBinding()]
	param(
		[Parameter()] [string]$Password,
		[Parameter()] [string]$PasswordFile
	)

	$args = @("encrypt", "--path", $script:sln_dir)
	$validation = $false

	if ([string]::IsNullOrEmpty($PasswordFile) -eq $false) {
		$args += "--password-file"
		$args += $PasswordFile
		$validation = $true
	}
	
	if ([string]::IsNullOrEmpty($Password) -eq $false) {
		$args += "--password"
		$args += $Password
		$validation = $true
	}

	if ($PSBoundParameters['Verbose']) {
		$args += "--verbose"
	}

	& $script:transformer_exe $args
}

Export-ModuleMember Switch-Environment
Export-ModuleMember Protect-Environments
Export-ModuleMember New-EncryptionKey
Export-ModuleMember Edit-EncryptionKey

Register-TabExpansion 'Switch-Environment' @{ 'environment' = { & $script:transformer_exe list } }
# Register-TabExpansion 'Switch-Environment' @{ 'environment' = { ((& $transformer_exe list) -split '[\r\n]') |? {$_} } } 