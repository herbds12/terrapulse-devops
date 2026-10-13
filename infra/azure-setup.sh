#!/usr/bin/env bash
# =====================================================================
# TerraPulse - Criacao da infraestrutura no Azure (staging + producao)
#
# Execute no Azure Cloud Shell (Bash) do portal: https://shell.azure.com
#   1) Envie este arquivo pelo botao "Upload" do Cloud Shell
#   2) bash azure-setup.sh
#
# Recursos criados:
#   - Resource Group
#   - App Service Plan Linux (B1) compartilhado
#   - 2 Web Apps for Containers: staging e producao
#   - Azure SQL Server + 2 bancos (staging e producao) na oferta gratuita
#   - Publish profiles (usados como secrets no GitHub Actions)
# =====================================================================
set -euo pipefail

# ---------- Parametros (ajuste se quiser) ----------
SUFIXO="${SUFIXO:-rm565128}"              # deixa os nomes globalmente unicos
LOCATION="${LOCATION:-chilecentral}"      # regiao do App Service (Azure for Students: ver "az policy assignment list")
# Regioes candidatas para o Azure SQL (tenta em ordem ate uma aceitar)
SQL_LOCATIONS="${SQL_LOCATIONS:-$LOCATION mexicocentral switzerlandnorth belgiumcentral southafricanorth}"
RG="rg-terrapulse-devops"
PLAN="plan-terrapulse"
APP_STG="terrapulse-stg-${SUFIXO}"
APP_PRD="terrapulse-prd-${SUFIXO}"
SQL_SERVER="sql-terrapulse-${SUFIXO}"
SQL_ADMIN="terrapulseadmin"
SQL_PASSWORD="${SQL_PASSWORD:-Tp$(openssl rand -hex 8)#2026}"
DB_STG="terrapulse-staging"
DB_PRD="terrapulse-production"
PLACEHOLDER_IMAGE="mcr.microsoft.com/appsvc/staticsite:latest"

echo ">> Registrando provedores (pode levar 1-2 min na primeira vez)"
az provider register --namespace Microsoft.Web --wait
az provider register --namespace Microsoft.Sql --wait

echo ">> Resource Group: $RG ($LOCATION)"
az group show -n "$RG" -o none 2>/dev/null || az group create -n "$RG" -l "$LOCATION" -o none

echo ">> App Service Plan Linux B1"
az appservice plan create -g "$RG" -n "$PLAN" --is-linux --sku B1 -l "$LOCATION" -o none

echo ">> Azure SQL Server"
if az sql server show -g "$RG" -n "$SQL_SERVER" -o none 2>/dev/null; then
  echo "   Ja existe, redefinindo a senha do administrador"
  az sql server update -g "$RG" -n "$SQL_SERVER" --admin-password "$SQL_PASSWORD" -o none
else
  SQL_OK=""
  for SQL_LOC in $SQL_LOCATIONS; do
    echo "   Tentando criar o SQL Server em: $SQL_LOC"
    if az sql server create -g "$RG" -n "$SQL_SERVER" -l "$SQL_LOC" \
         --admin-user "$SQL_ADMIN" --admin-password "$SQL_PASSWORD" -o none; then
      SQL_OK="$SQL_LOC"; break
    fi
    echo "   Regiao $SQL_LOC nao aceitou o Azure SQL, tentando a proxima..."
    az sql server delete -g "$RG" -n "$SQL_SERVER" --yes -o none 2>/dev/null || true
  done
  if [ -z "$SQL_OK" ]; then
    echo "!! Nenhuma regiao aceitou o Azure SQL. Envie este log para analise."; exit 1
  fi
  echo "   SQL Server criado em: $SQL_OK"
fi

az sql server firewall-rule create -g "$RG" -s "$SQL_SERVER" \
  -n AllowAzureServices --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0 -o none

criar_banco () {
  local DB="$1"
  if az sql db show -g "$RG" -s "$SQL_SERVER" -n "$DB" -o none 2>/dev/null; then
    echo ">> Banco $DB ja existe"; return
  fi
  echo ">> Banco $DB (oferta gratuita serverless)"
  if ! az sql db create -g "$RG" -s "$SQL_SERVER" -n "$DB" \
        --edition GeneralPurpose --compute-model Serverless --family Gen5 --capacity 2 \
        --use-free-limit --free-limit-exhaustion-behavior AutoPause -o none; then
    echo "   Oferta gratuita indisponivel, criando no tier Basic (~US\$5/mes)"
    az sql db create -g "$RG" -s "$SQL_SERVER" -n "$DB" --service-objective Basic -o none
  fi
}
criar_banco "$DB_STG"
criar_banco "$DB_PRD"

conn_string () {
  echo "Server=tcp:${SQL_SERVER}.database.windows.net,1433;Initial Catalog=$1;User ID=${SQL_ADMIN};Password=${SQL_PASSWORD};Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;"
}

criar_webapp () {
  local APP="$1" AMBIENTE="$2" DB="$3"
  echo ">> Web App $APP ($AMBIENTE)"
  if ! az webapp show -g "$RG" -n "$APP" -o none 2>/dev/null; then
    az webapp create -g "$RG" -p "$PLAN" -n "$APP" --container-image-name "$PLACEHOLDER_IMAGE" -o none
  fi

  az webapp config set -g "$RG" -n "$APP" --always-on true --http20-enabled true -o none
  az webapp update -g "$RG" -n "$APP" --https-only true -o none

  az webapp config appsettings set -g "$RG" -n "$APP" -o none --settings \
    WEBSITES_PORT=8080 \
    ASPNETCORE_ENVIRONMENT="$AMBIENTE" \
    Database__ApplyMigrations=true \
    Swagger__Enabled=true \
    Jwt__Issuer=TerraPulse.Api \
    Jwt__Audience=TerraPulse.Client \
    Jwt__Key="$(openssl rand -base64 48 | tr -d '\n')" \
    ConnectionStrings__DefaultConnection="$(conn_string "$DB")"

  # Publish profile exige "SCM Basic Auth" habilitado (desligado por padrao em apps novos)
  az resource update -g "$RG" --namespace Microsoft.Web --resource-type basicPublishingCredentialsPolicies \
    --parent "sites/$APP" -n scm --set properties.allow=true -o none

  az webapp log config -g "$RG" -n "$APP" --docker-container-logging filesystem -o none

  az webapp deployment list-publishing-profiles -g "$RG" -n "$APP" --xml > "publish-profile-${AMBIENTE,,}.xml"
}
criar_webapp "$APP_STG" "Staging"    "$DB_STG"
criar_webapp "$APP_PRD" "Production" "$DB_PRD"

cat <<EOF

=====================================================================
 Infraestrutura criada!

 Staging : https://${APP_STG}.azurewebsites.net
 Producao: https://${APP_PRD}.azurewebsites.net
 SQL     : ${SQL_SERVER}.database.windows.net  (usuario: ${SQL_ADMIN})
 Senha SQL (guarde, NAO versionar): ${SQL_PASSWORD}

 Proximos passos no GitHub (Settings > Secrets and variables > Actions):
   Variables:
     AZURE_WEBAPP_STAGING    = ${APP_STG}
     AZURE_WEBAPP_PRODUCTION = ${APP_PRD}
   Secrets (cole o conteudo inteiro dos arquivos):
     AZURE_PUBLISH_PROFILE_STAGING    = cat publish-profile-staging.xml
     AZURE_PUBLISH_PROFILE_PRODUCTION = cat publish-profile-production.xml
=====================================================================
EOF
