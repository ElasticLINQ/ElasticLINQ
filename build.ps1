param(
    [string]$target = "Default",
    [string]$verbosity = "minimal",
    [string]$properties = "",
    [int]$maxCpuCount = 0
)

$msbuilds = @(get-command msbuild -ea SilentlyContinue)
if ($msbuilds.Count -eq 0) {
    $msbuild = join-path $env:windir "Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
} else {
    $msbuild = $msbuilds[0].Definition
}

if ($maxCpuCount -lt 1) {
    $maxCpuCountText = $Env:MSBuildProcessorCount
} else {
    $maxCpuCountText = ":$maxCpuCount"
}

if ($properties -ne "") {
    $properties = "/p:" + $properties
}

$allArgs = @("Build\Build.proj", "/m$maxCpuCountText", "/nologo", "/verbosity:$verbosity", "/t:$target", $properties)
& $msbuild $allArgs
