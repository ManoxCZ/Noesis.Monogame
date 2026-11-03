@echo off

dotnet publish Noesis.MonoGame\Noesis.MonoGame.csproj
dotnet publish Noesis.MonoGame.Content\Noesis.MonoGame.Content.csproj

cd Noesis.MonoGame.Content\bin\Release

REM Noesis.MonoGame.Content.exe build -p DesktopGL -s ..\..\Assets -o DesktopGL
REM xcopy DesktopGL\Content\Noesis\Effects ..\..\..\NugetPackage\Content\Noesis\Effects\DesktopGL /s /e /I

REM Noesis.MonoGame.Content.exe build -p DesktopVk -s ..\..\Assets -o DesktopVk
REM xcopy DesktopVk\Content\Noesis\Effects ..\..\..\NugetPackage\Content\Noesis\Effects\DesktopVk /s /e /I

REM Noesis.MonoGame.Content.exe build -p WindowsDX12 -s ..\..\Assets -o WindowsDX12
REM xcopy WindowsDX12\Content\Noesis\Effects ..\..\..\NugetPackage\Content\Noesis\Effects\WindowsDX12 /s /e /I

cd ..\..\..
robocopy "." "NugetPackage" Noesis.MonoGame.nuspec /R:1 /W:1
robocopy "." "NugetPackage" README.md /R:1 /W:1
pushd NugetPackage
ren README.md readme
popd

dotnet pack NugetPackage\Noesis.MonoGame.nuspec
