@echo off
chcp 1251 > nul
echo Запуск интернет-магазина...
cd bin\Debug\net6.0-windows
start ProjectStore.exe
pause