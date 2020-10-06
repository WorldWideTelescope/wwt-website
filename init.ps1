param ([switch]$restore)

if ($restore) {
  Push-Location src/WWTMVC5
  npm install
  if (-not $?) { throw "npm install failed with error code $LastExitCode" }
  Pop-Location
}

Push-Location src/WWTMVC5
bower install
if (-not $?) { throw "bower install failed with error code $LastExitCode" }

grunt copy
if (-not $?) { throw "grunt copy failed with error code $LastExitCode" }
Pop-Location