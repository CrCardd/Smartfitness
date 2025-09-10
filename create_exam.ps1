$folder = ".\$args"
$zipped = ".\$args.zip"

Compress-Archive -Path "$folder\*" -DestinationPath $zipped -Force

Move-Item .\$args.zip .\$args.exam -Force

dotnet run