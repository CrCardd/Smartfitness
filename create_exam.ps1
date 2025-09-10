$folder = ".\lp_exam"
$zipped = ".\lp.zip"

Compress-Archive -Path "$folder\*" -DestinationPath $zipped -Force

Move-Item .\lp.zip .\lp.exam -Force

dotnet run