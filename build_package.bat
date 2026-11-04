@echo off

dotnet publish Noesis.MonoGame\Noesis.MonoGame.csproj
dotnet publish Noesis.MonoGame.Content\Noesis.MonoGame.Content.csproj

xcopy "Noesis.MonoGame\bin\Release\net10.0\publish\Content\" "Content\" /s /e /I
dotnet pack ManoxCZ.Noesis.MonoGame.nuspec

rd /S /Q Content
