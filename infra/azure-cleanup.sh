#!/usr/bin/env bash
# Remove TODOS os recursos criados (use depois da correcao para nao gastar creditos)
set -euo pipefail
az group delete -n rg-terrapulse-devops --yes --no-wait
echo "Exclusao do resource group iniciada."
