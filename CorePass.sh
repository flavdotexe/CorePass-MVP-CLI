#!/bin/bash
# ===============================================================================================
PROJECT_PATH="$HOME/your/path/data/CorePass-MVP-CLI/CorePass.CLI/CorePass.CLI.csproj"

if ! command -v dotnet &> /dev/null
then
    echo "Dotnet not found! Install .NET 8 SDK first."
    exit 1
fi

echo "Run CorePass CLI..."
dotnet run -c Release --project "$PROJECT_PATH" 2> /dev/null
# =================================================================================================
