param ([switch]$restore)

if ($restore) {
  Push-Location WWTMVC5
  npm install
  Pop-Location
}

Push-Location WWTMVC5
bower install
grunt copy

Pop-Location