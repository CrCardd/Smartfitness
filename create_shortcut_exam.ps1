$path = $args[0] + "\exam"
$abspath = (Get-Location).Path

New-Item -Path (".\" + $path + "\details") -ItemType Directory > $null

New-Item -Path (".\" + $path + "\details\exam.data") -Value "DataPath = $abspath\$args" > $null

New-Item -Path (".\" + $path + "\compress_to_exam.ps1") -Value @'
$folder = ".\details"
$zipped = ".\details.zip"
Compress-Archive -Path "$folder\*" -DestinationPath $zipped -Force
Move-Item $zipped .\.exam -Force
'@ > $null


$batabspath = $abspath + "\" + $args + "\exam"

New-Item -Path (".\" + $path + "\.ps1") -Value @"
Set-Location $batabspath > `$null
& ../../Smartfitness/Smartfitness.exe > `$null
"@ > $null


$batabspath = $batabspath -replace '\\', '\\'

New-Item -Path "./short_cut.c" -Value @"
#include <stdlib.h>

int main() {
    system("powershell.exe \"$batabspath\\.ps1\""); 
    return 0;
}
"@ > $null

gcc ./short_cut.c -o "$args/$args.exe"

Remove-Item ./short_cut.c