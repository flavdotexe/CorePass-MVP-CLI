#!/bin/bash
# =========================================
# CorePass CLI launcher - Linux
# =========================================

# Caminho absoluto do projeto CorePass.CLI.csproj
PROJECT_PATH="$HOME/Área de trabalho/Workspace/CorePass-MVP-CLI/CorePass.CLI/CorePass.CLI.csproj"

# Checa se dotnet está instalado
if ! command -v dotnet &> /dev/null
then
    echo "dotnet não encontrado! Instale o .NET 8 SDK primeiro."
    exit 1
fi

# Executa o projeto em Release
echo "Rodando CorePass CLI..."
dotnet run -c Release --project "$PROJECT_PATH" 2> /dev/null
