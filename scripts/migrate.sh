#!/usr/bin/env bash

MIGRATION_NAME="$1";
DB_CONTEXT="${2:-AppDbContext}";
CONF_ENV="${3:-Development}";

ROOT_NS='Plainly';
MIGRATIONS_PROJECT="$ROOT_NS.Infrastructure";

dotnet ef migrations add "$MIGRATION_NAME" \
  --startup-project "../src/$ROOT_NS/$ROOT_NS.csproj" \
  --project         "../src/$MIGRATIONS_PROJECT/$MIGRATIONS_PROJECT.csproj" \
  --output-dir      'Data/Migrations' \
  --context         "$MIGRATIONS_PROJECT.Data.$DB_CONTEXT" \
  --verbose \
  -- "$CONF_ENV";

echo "Done adding migration $MIGRATION_NAME";

dotnet ef database update \
  --startup-project "../src/$ROOT_NS/$ROOT_NS.csproj" \
  -- "$CONF_ENV"
  
echo "Done updating database with migration $MIGRATION_NAME";




