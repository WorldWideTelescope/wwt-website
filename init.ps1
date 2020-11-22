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

grunt vendor dist-css
if (-not $?) { throw "grunt failed with error code $LastExitCode" }
Pop-Location