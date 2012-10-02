function Get-PsTestModulePath {

    #
    # .SYNOPSIS
    # Get the path to the PsTest binary module assembly.
    #

    [CmdletBinding()]
    param (
    
        #
        # The path to the tools folder of the PsTest NuGet package.
        #
    
        [Parameter(
            Mandatory = $true,
            ValueFromPipeline = $true
        )]
        [string] $ToolsPath
    )
    
    process {
        $ToolsPath | Join-Path -ChildPath pstest.dll
    }
}

# -----------------------------------------------------------------------------

function Get-AssemblyVersion {

    #
    # .SYNOPSIS
    # Get the assembly version.
    #
    
    [CmdletBinding()]
    param (
    
        #
        # Assembly path.
        #
        [Parameter(
            Mandatory = $true,
            ValueFromPipeline = $true
        )]
        [string] $Path
    )
    
    process {
        ($Path | Get-Item).VersionInfo.ProductVersion
    }
}

# -----------------------------------------------------------------------------

function Get-PsTestTempPath {

    #
    # .SYNOPSIS
    # Get the PsTest temp path.
    #
    
    [CmdletBinding()]
    param (
    
        #
        # The PsTest binary module assembly version.
        #
        [Parameter(
            Mandatory = $true,
            ValueFromPipeline = $true
        )]
        [string] $AssemblyVersion
    )
    
    process {
        [IO.Path]::GetTempPath() |
            Join-Path -ChildPath "PsTest\$AssemblyVersion"
    }
}

# -----------------------------------------------------------------------------

function Copy-PsTestAssembly {

    #
    # .SYNOPSIS
    # Copies the PsTest assembly to the PsTest temp path.
    #
    
    [CmdletBinding()]
    param (
    
        #
        # The path to the tools folder of the PsTest NuGet package.
        #
        [Parameter(
            Mandatory = $true,
            ValueFromPipeline = $true
        )]
        [string] $ToolsPath
    )

    process {
        $psTestModulePath = $ToolsPath | Get-PsTestModulePath
        $psTestTempPath =
            $psTestModulePath | Get-AssemblyVersion | Get-PsTestTempPath
        if (-not ($psTestTempPath | Test-Path)) {
            New-Item -ItemType Directory -Path $psTestTempPath | Out-Null
        }
        $psTestTempAssemblyPath =
            $psTestTempPath | Join-Path -ChildPath pstest.dll
        if (-not ($psTestTempAssemblyPath | Test-Path)) {
            Copy-Item -Path $psTestModulePath -Destination $psTestTempPath
        }
        $psTestTempAssemblyPath
    }
}