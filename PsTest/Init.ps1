#
# .SYNOPSIS
# Imports the PsTest module.
#

param (
	$InstallPath,
	$ToolsPath,
	$Package,
	$Project
)

$modulePath = $ToolsPath | Join-Path -ChildPath PsTest
Import-Module -Name $modulePath