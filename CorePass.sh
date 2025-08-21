#!/bin/bash
# ===============================================================================================
PROJECT_PATH="/home/your/path/CorePass-MVP-CLI-main/CorePass.CLI/CorePass.CLI.csproj"

if ! command -v dotnet &> /dev/null
then
    echo "Dotnet not found! Install .NET 8 SDK first."
    exit 1
fi

echo "Run CorePass CLI..."
dotnet run -c Release --project "$PROJECT_PATH" 2> /dev/null
# =================================================================================================
