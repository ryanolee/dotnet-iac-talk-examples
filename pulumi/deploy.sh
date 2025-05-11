#!/usr/bin/env bash

# Run in strict mode
set -euo pipefail

PULUMI_STACK="dev"
export PULUMI_CONFIG_PASSPHRASE=""

cd "./infra"
pulumi stack select "${PULUMI_STACK}"
 pulumi up --yes --skip-preview --non-interactive --stack "${PULUMI_STACK}" 
FUNCTION_NAME=$(pulumi stack output functionName)

echo "Function name: ${FUNCTION_NAME}"
echo "Deploying to Azure Function App: ${FUNCTION_NAME}"

# Check if the function name is empty
if [ -z "${FUNCTION_NAME}" ]; then
  echo "Error: Function name is empty. Please check your Pulumi configuration."
  exit 1
fi

cd "../DotNetBrumFunc"

func azure functionapp publish ${FUNCTION_NAME}

