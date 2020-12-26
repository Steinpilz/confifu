SET TOOL_PATH=.fake

for /f "delims== tokens=1,2" %%G in (.env) do call set "%%G=%%H"

IF NOT EXIST "%TOOL_PATH%\fake.exe" (
  dotnet tool install fake-cli --tool-path ./%TOOL_PATH%
)

"%TOOL_PATH%/fake.exe" run scripts/build.fsx %*
