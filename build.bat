@echo off
cls

for /f "delims== tokens=1,2" %%G in (.env) do call set "%%G=%%H"

if not exist "packages" (
	.paket\paket.exe restore
	if errorlevel 1 (
	exit /b %errorlevel%
	)
)


"packages\FAKE\tools\Fake.exe" scripts\build.fsx %*