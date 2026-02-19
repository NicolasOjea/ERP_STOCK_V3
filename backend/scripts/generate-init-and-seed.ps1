param(
    [string]$Project = "backend/src/Pos.Infrastructure",
    [string]$StartupProject = "backend/src/Pos.WebApi",
    [string]$OutDir = "backend/scripts/sql"
)

$ErrorActionPreference = "Stop"

New-Item -ItemType Directory -Force -Path $OutDir | Out-Null

$fullPath = Join-Path $OutDir "full-model.sql"
$initPath = Join-Path $OutDir "init.sql"
$seedPath = Join-Path $OutDir "seed.sql"
$resetPath = Join-Path $OutDir "reset.sql"

# Generates SQL for current EF model (DDL + HasData inserts)
dotnet dotnet-ef dbcontext script -p $Project -s $StartupProject --output $fullPath | Out-Null

$full = Get-Content -Raw -Path $fullPath

# Capture INSERT blocks as optional seed
$insertMatches = [regex]::Matches($full, '(?ms)^INSERT INTO\s+.+?;\s*$')
$seedBlocks = @()
foreach ($m in $insertMatches) { $seedBlocks += $m.Value.TrimEnd() }

$seedHeader = @(
    '-- Optional seed data generated from EF HasData'
    '-- Execute after init.sql'
    ''
)

if ($seedBlocks.Count -gt 0) {
    ($seedHeader + $seedBlocks) -join "`r`n`r`n" | Set-Content -Path $seedPath -Encoding UTF8
}
elseif (-not (Test-Path $seedPath)) {
    ($seedHeader + '-- No HasData inserts found.') -join "`r`n" | Set-Content -Path $seedPath -Encoding UTF8
}

# Remove inserts from full script -> pure DDL init
$init = [regex]::Replace($full, '(?ms)^INSERT INTO\s+.+?;\s*$', '')
$init = [regex]::Replace($init, '(?m)^[ \t]*\r?\n', '')

$ddlPreamble = @'
-- Extra DDL required by model defaults/custom SQL migrations
CREATE SEQUENCE IF NOT EXISTS venta_numero_seq START WITH 1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE CACHE 1;

'@

Set-Content -Path $initPath -Value ($ddlPreamble + $init) -Encoding UTF8

@"
-- WARNING: drops everything in public schema
DROP SCHEMA IF EXISTS public CASCADE;
CREATE SCHEMA public;
"@ | Set-Content -Path $resetPath -Encoding UTF8

Write-Host "Generated:"
Write-Host " - $initPath"
Write-Host " - $seedPath"
Write-Host " - $fullPath"
Write-Host " - $resetPath"
