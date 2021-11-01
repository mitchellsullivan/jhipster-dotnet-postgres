#!/usr/bin/env bash

MIGRATION_NAME="$1";
DB_CONTEXT="${2:-AppDbContext}";
CONF_ENV="${3:-Development}";

ROOT_NS='Plainly';
MIGRATIONS_PROJECT="$ROOT_NS.Infrastructure";

dotnet ef database update \
  --startup-project "../src/$ROOT_NS/$ROOT_NS.csproj" \
  -- "$CONF_ENV"

echo "Done updating database with migration $MIGRATION_NAME";




