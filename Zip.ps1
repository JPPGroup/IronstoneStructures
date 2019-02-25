Get-ChildItem -Include *IronstoneStructures.dll -Exclude *Tests.dll -Recurse | Compress-Archive -Update -DestinationPath ($PSScriptRoot + "\IronstoneStructures")
Write-Host "Zip file created"