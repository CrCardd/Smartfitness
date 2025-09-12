dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true

Move-Item .\bin\Release\net9.0-windows\win-x64\publish\Smartfitness.exe .\ -Force