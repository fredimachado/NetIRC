dotnet restore NetIRC.sln
dotnet build NetIRC.sln --no-incremental -c Release /p:codecov=1

nuget restore .\tests\CodeCoverage -PackagesDirectory .\tests\CodeCoverage

.\tests\CodeCoverage\OpenCover.4.6.519\tools\OpenCover.Console.exe -register:user -target:"dotnet.exe" -targetargs:"test tests\NetIRC.Tests\NetIRC.Tests.csproj --no-build -c release /p:codecov=1" -hideskipped:All -oldStyle -output:.\NetIRC_coverage.xml

$env:PATH = "C:\Python36;C:\Python36\Scripts\;$env:PATH"

pip install codecov
codecov -f "NetIRC_coverage.xml" -X gcov
