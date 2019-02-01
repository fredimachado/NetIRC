$commitHash = $env:BUILD_SOURCEVERSION.Substring(0, 7)

Write-Host "Branch name: $env:BUILD_SOURCEBRANCHNAME"
Write-Host "Commit hash: $commitHash"
Write-Host "Build number: $env:BUILD_BUILDNUMBER"
