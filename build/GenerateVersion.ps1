$commitHash = $env:BUILD_SOURCEVERSION.Substring(0, 7)

Write-Host "Branch name: $env:BUILD_SOURCEBRANCHNAME"
Write-Host "Commit hash: $commitHash"
Write-Host "Build number: $env:BUILD_BUILDNUMBER"

$versionSufix = "master-$commitHash"

if ($env:BUILD_REASON -eq "PullRequest") {
    $versionSufix = "$env:BUILD_BUILDNUMBER-$commitHash"
    $packageVersionSufix = "$env:BUILD_BUILDNUMBER-PR-$env:SYSTEM_PULLREQUEST_PULLREQUESTNUMBER"
    Write-Host "Package version sufix: $packageVersionSufix"
    Write-Host "##vso[task.setvariable variable=PackageVersionSufix]--version-suffix=$packageVersionSufix"
}

Write-Host "Build version sufix: $versionSufix"
Write-Host "##vso[task.setvariable variable=VersionSufix]$versionSufix"
