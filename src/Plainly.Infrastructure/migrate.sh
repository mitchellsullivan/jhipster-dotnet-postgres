#!/usr/bin/env bash


dotnet ef migrations add \
  --project Plainly.Infrastructure.csproj \
  --context Plainly.Infrastructure.Data.AppDbContext \
  --startup-project ../Plainly/Plainly.csproj \
  -o ./Data/Migrations \
  --verbose \
  "$1" -- Development

echo "Done adding migration $1"

dotnet ef \
  --startup-project ../Plainly/Plainly.csproj \
  database update -- Development
  
echo "Done updating database with migration $1"




