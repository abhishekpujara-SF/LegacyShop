@echo off
rem Creates and seeds the LegacyShop database. Usage: setup.cmd [server]   (default: .)
set SRV=%1
if "%SRV%"=="" set SRV=.
sqlcmd -S %SRV% -E -b -i "%~dp001_schema.sql" || exit /b 1
sqlcmd -S %SRV% -E -b -i "%~dp002_seed.sql"   || exit /b 1
echo LegacyShop database ready on %SRV%.
