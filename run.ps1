clear
cd FitnessTest
dotnet build
cd ..

cp .\FitnessTest\bin\Debug\net7.0-windows\pamella.dll .\test\pamella.dll
cp .\FitnessTest\bin\Debug\net7.0-windows\Stately.dll .\test\Stately.dll
cp .\FitnessTest\bin\Debug\net7.0-windows\FitnessTest.dll .\test\FitnessTest.dll
cp .\FitnessTest\bin\Debug\net7.0-windows\FitnessTest.exe .\test\FitnessTest.exe
cp .\FitnessTest\bin\Debug\net7.0-windows\FitnessTest.pdb .\test\FitnessTest.pdb
cp .\FitnessTest\bin\Debug\net7.0-windows\FitnessTest.deps.json .\test\FitnessTest.deps.json
cp .\FitnessTest\bin\Debug\net7.0-windows\FitnessTest.runtimeconfig.json .\test\FitnessTest.runtimeconfig.json
cp (".\testExamples\" + $args[0].ToString() + ".exam") (".\test\" + $args[0].ToString() + ".exam")

cd test
sc ".name" "tester"
.\FitnessTest.exe
cd ..