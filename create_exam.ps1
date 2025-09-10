$folder = ".\$args"
$zipped = ".\$args.zip"

Compress-Archive -Path "$folder\*" -DestinationPath $zipped -Force

Move-Item .\lp.zip .\lp.exam -Force

dotnet run