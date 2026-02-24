@echo off
REM Add Souqna.Application project to solution
cd /d "D:\Desktop\Souqna\Souqna"
dotnet sln Souqna.sln add Souqna.Application\Souqna.Application.csproj
dotnet restore
dotnet build
pause
