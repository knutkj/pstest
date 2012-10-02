Clear-Host

'NUnit', 'PsTest' |
    Where-Object -FilterScript { ($_ | Get-Module) -eq $null } |
    Import-Module
    
. .\InitBase.ps1

# -----------------------------------------------------------------------------

#
# Get-PsTestModulePath tests...
#
(New-Test 'Get-PsTestModulePath: works' {

    # Arrange.
    $expectedToolsPath = 'tools path'
    $expectedJoinedPath = 'joined path'
    
    $joinPathCalled = $false

    function Join-Path (
        [Parameter(ValueFromPipeline = $true)] $Path,
        $ChildPath
    )
    {
        Set-Variable -Name joinPathCalled -Value $true -Scope 2
        $Assert::That($Path, $Is::EqualTo($expectedToolsPath))
        $Assert::That($ChildPath, $Is::EqualTo('pstest.dll'))
        $expectedJoinedPath
    }
    
    # Act.
    $actualJoinedPath = Get-PsTestModulePath -ToolsPath $expectedToolsPath
    
    # Assert.
    $Assert::IsTrue($joinPathCalled)
    $Assert::That($actualJoinedPath, $Is::EqualTo($expectedJoinedPath))

}),

# -----------------------------------------------------------------------------

#
# Get-AssemblyVersion tests...
#
(New-Test 'Get-AssemblyVersion: works' {

    # Arrange.
    $assemblyPath = 'path to assembly'
    $expectedVersion = 'expected version'
    
    $getItemCalled = $false
    
    function Get-Item (
        [Parameter(ValueFromPipeline = $true)] $Path
    )
    {
        Set-Variable -Name getItemCalled -Value $true -Scope 2
        $Assert::That($Path, $Is::EqualTo($assemblyPath))
        New-Object -TypeName PSObject -Property @{
            VersionInfo = (New-Object -TypeName PSObject -Property @{
                ProductVersion = $expectedVersion
            })
        }
    }
    
    # Act.
    $actualVersion = Get-AssemblyVersion -Path $assemblyPath
    
    # Assert.
    $Assert::IsTrue($getItemCalled)
    $Assert::That($actualVersion, $Is::EqualTo($expectedVersion))
    
}),

# -----------------------------------------------------------------------------

#
# Get-PsTestTempPath tests...
#
(New-Test 'Get-PsTestTempPath: works' {

    # Arrange.
    $assemblyVersion = 'assembly version'
    $expectedPath = 'expected path'
    
    $joinPathCalled = $false

    function Join-Path (
        [Parameter(ValueFromPipeline = $true)] $Path,
        $ChildPath
    )
    {
        Set-Variable -Scope 2 -Name joinPathCalled -Value $true
        $Assert::That($Path, $Is::EqualTo([IO.Path]::GetTempPath()))
        $Assert::That($ChildPath, $Is::EqualTo("PsTest\$assemblyVersion"))
        $expectedPath
    }
    
    # Act.
    $actualPath = $assemblyVersion | Get-PsTestTempPath
    
    # Assert.
    $Assert::IsTrue($joinPathCalled)
    $Assert::That($actualPath, $Is::EqualTo($expectedPath))

}),

# -----------------------------------------------------------------------------

#
# Copy-PsTestAssembly tests...
#
(New-Test 'Copy-PsTestAssembly: creates directory and copies assembly' {

    # Arrange.
    $expectedToolsPath = 'tools path'
    $expectedModulePath = 'module path'
    $expectedAssemblyVersion = 'assembly version'
    $expectedPsTestTempPath = 'temp path'
    $expectedPsTestAssemblyDestinationPath = 'destination path'
    
    $getPsTestModulePathCalled = $false
    $getAssemblyVersionCalled = $false
    $getPsTestTempPathCalled = $false
    $testPathCalled = $false
    $testPath2Called = $false
    $newItemCalled = $false
    $joinPathCalled = $false
    $copyItemCalled = $false
    
    function Get-PsTestModulePath (
        [Parameter(ValueFromPipeline = $true)] $ToolsPath
    )
    {
        Set-Variable -Scope 2 -Name getPsTestModulePathCalled -Value $true
        $Assert::That($ToolsPath, $Is::EqualTo($expectedToolsPath))
        $expectedModulePath
    }
    
    function Get-AssemblyVersion (
        [Parameter(ValueFromPipeline = $true)] $Path
    )
    {
        Set-Variable -Scope 2 -Name getAssemblyVersionCalled -Value $true
        $Assert::That($Path, $Is::EqualTo($expectedModulePath))
        $expectedAssemblyVersion
    }
    
    function Get-PsTestTempPath (
        [Parameter(ValueFromPipeline = $true)] $AssemblyVersion
    )
    {
        Set-Variable -Scope 2 -Name getPsTestTempPathCalled -Value $true
        $Assert::That($AssemblyVersion, $Is::EqualTo($expectedAssemblyVersion))
        $expectedPsTestTempPath
    }
    
    function Test-Path (
        [Parameter(ValueFromPipeline = $true)] $Path
    )
    {
        # Testing for temp directory.
        if (-not $testPathCalled) {
            Set-Variable -Scope 2 -Name testPathCalled -Value $true
            $Assert::That($Path, $Is::EqualTo($expectedPsTestTempPath))
            $false
            
        # Testing for temp assembly.
        } elseif (-not $testPath2Called) {
            Set-Variable -Scope 2 -Name testPath2Called -Value $true
            $Assert::That(
                $Path,
                $Is::EqualTo($expectedPsTestAssemblyDestinationPath)
            )
            $false
        }
    }
    
    function New-Item (
        $ItemType,
        $Path
    )
    {
        Set-Variable -Scope 2 -Name newItemCalled -Value $true
        $Assert::That($ItemType, $Is::EqualTo('Directory'))
        $Assert::That($Path, $Is::EqualTo($expectedPsTestTempPath))
    }
    
    function Join-Path (
        [Parameter(ValueFromPipeline = $true)] $Path,
        $ChildPath
    )
    {
        Set-Variable -Scope 2 -Name joinPathCalled -Value $true
        $Assert::That($Path, $Is::EqualTo($expectedPsTestTempPath))
        $Assert::That($ChildPath, $Is::EqualTo('pstest.dll'))
        $expectedPsTestAssemblyDestinationPath
    }
    
    function Copy-Item ($Path, $Destination) {
        Set-Variable -Scope 2 -Name copyItemCalled -Value $true
        $Assert::That($Path, $Is::EqualTo($expectedModulePath))
        $Assert::That($Destination, $Is::EqualTo($expectedPsTestTempPath))
    }
    
    # Act.
    $expectedToolsPath | Copy-PsTestAssembly
    
    # Assert.
    $Assert::IsTrue($getPsTestModulePathCalled)
    $Assert::IsTrue($getAssemblyVersionCalled)
    $Assert::IsTrue($getPsTestTempPathCalled)
    $Assert::IsTrue($testPathCalled)
    $Assert::IsTrue($testPath2Called)
    $Assert::IsTrue($newItemCalled)
    $Assert::IsTrue($joinPathCalled)
    $Assert::IsTrue($copyItemCalled)

}),

(New-Test 'Copy-PsTestAssembly: directory exists copies assembly' {

    # Arrange.
    $toolsPath = 'tools path'
    $modulePath = 'module path'
    $assemblyVersion = 'assembly version'
    $psTestTempPath = 'temp path'
    $psTestAssemblyDestinationPath = 'destination path'

    $testPathCalled = $false
    $copyItemCalled = $false
    
    function Get-PsTestModulePath (
        [Parameter(ValueFromPipeline = $true)] $ToolsPath
    )
    { $modulePath }
    
    function Get-AssemblyVersion (
        [Parameter(ValueFromPipeline = $true)] $Path
    )
    { $assemblyVersion }
    
    function Get-PsTestTempPath (
        [Parameter(ValueFromPipeline = $true)] $AssemblyVersion
    )
    { $psTestTempPath }
    
    function Test-Path (
        [Parameter(ValueFromPipeline = $true)] $Path
    )
    {
        if (-not $testPathCalled) { # Testing for temp directory.
            Set-Variable -Scope 2 -Name testPathCalled -Value $true
            $true
        } else { $false } # Testing for temp assembly.
    }
    
    function New-Item () { throw 'Should not create directory...' }
    
    function Join-Path (
        [Parameter(ValueFromPipeline = $true)] $Path,
        $ChildPath
    )
    { $psTestAssemblyDestinationPath }
    
    function Copy-Item ($Path, $Destination) {
        Set-Variable -Scope 2 -Name copyItemCalled -Value $true
        $Assert::That($Path, $Is::EqualTo($modulePath))
        $Assert::That($Destination, $Is::EqualTo($psTestTempPath))
    }
    
    # Act.
    $toolsPath | Copy-PsTestAssembly
    
    # Assert.
    $Assert::IsTrue($copyItemCalled)

}),

(New-Test 'Copy-PsTestAssembly: directory and assembly exists' {

    # Arrange.
    $toolsPath = 'tools path'
    $modulePath = 'module path'
    $assemblyVersion = 'assembly version'
    $psTestTempPath = 'temp path'
    $psTestAssemblyDestinationPath = 'destination path'
    
    function Get-PsTestModulePath (
        [Parameter(ValueFromPipeline = $true)] $ToolsPath
    )
    { $modulePath }
    
    function Get-AssemblyVersion (
        [Parameter(ValueFromPipeline = $true)] $Path
    )
    { $assemblyVersion }
    
    function Get-PsTestTempPath (
        [Parameter(ValueFromPipeline = $true)] $AssemblyVersion
    )
    { $psTestTempPath }
    
    function Test-Path (
        [Parameter(ValueFromPipeline = $true)] $Path
    )
    { $true }
    
    function New-Item () { throw 'Should not create directory...' }
    
    function Join-Path (
        [Parameter(ValueFromPipeline = $true)] $Path,
        $ChildPath
    )
    { $psTestAssemblyDestinationPath }
    
    function Copy-Item () { throw 'Should not copy assembly...' }
    
    # Act and assert.
    $copyPath = $toolsPath | Copy-PsTestAssembly
    
    # Assert.
    $Assert::That($copyPath, $Is::EqualTo($psTestAssemblyDestinationPath))

}) |

# -----------------------------------------------------------------------------

# Invoke tests.
Invoke-Test |

# Format test results.
Format-TestResult -All