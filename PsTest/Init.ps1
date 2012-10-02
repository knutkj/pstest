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

. ($ToolsPath | Join-Path -ChildPath InitBase.ps1)
$ToolsPath | Copy-PsTestAssembly | Import-Module