#!/usr/bin/env bash

MIGRATION_NAME="$1";
DB_CONTEXT="$2";
CONF_ENV="${3:-$Development}";

SLN='Plainly';
MIGRATIONS_PROJECT="$SLN.Infrastructure";

dotnet ef migrations add "$MIGRATION_NAME" \
  --project "../src/$MIGRATIONS_PROJECT/$SLN.Infrastructure.csproj" \
  --context "../src/$MIGRATIONS_PROJECT/$MIGRATIONS_PROJECT.Data.$DB_CONTEXT" \
  --output-dir "../src/$MIGRATIONS_PROJECT/Data/Migrations" \
  --startup-project "../src/$SLN/$SLN.csproj" \
  --verbose \
  -- "$CONF_ENV";

echo "Done adding migration $MIGRATION_NAME";

dotnet ef database update \
  --startup-project ../Plainly/Plainly.csproj \
   -- "$CONF_ENV"
  
echo "Done updating database with migration $MIGRATION_NAME"




