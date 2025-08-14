@echo off
REM ========================================
REM CorePass Run Script (Windows)
REM Edite o caminho do projeto abaixo
REM ========================================

SET PROJECT_PATH=C:\Users\your\path\data\CorePass-MVP-CLI\CorePass.CLI\CorePass.CLI.csproj

echo Running CorePass CLI...
dotnet run -c Release --project "%PROJECT_PATH%"

pause
