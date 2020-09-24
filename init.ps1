param ([switch]$restore)

if ($restore) {
  Push-Location src/WWTMVC5
  npm install
  Pop-Location
}

Push-Location src/WWTMVC5
bower install
grunt copy

Pop-Location