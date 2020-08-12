param ([switch]$restore)

if ($restore) {
  $nuget = ".nuget/nuget.exe";

  if (!(Test-Path $nuget)) {
    New-Item -ItemType Directory .nuget -ErrorAction Ignore | Out-Null
    Write-Host "Downloading nuget"
    Invoke-WebRequest "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile $nuget
  }

  & $nuget restore .\WWTMVC5.sln

  Push-Location WWTMVC5
  npm install
  Pop-Location
}

Push-Location WWTMVC5
bower install
grunt copy

Copy-Item -Recurse ..\packages\WURFL_Official_API.1.9.0.1\Content\* .

Pop-Location