@echo off
title Copy NuGets to Local Testing

set "projectdirectory=%~1"

rem Change to the package directory.
chdir /d "%projectdirectory%/bin/Release"
echo.
echo Processing the directory: %cd%

rem Set the destination.
set "destination=..\..\..\..\..\NuGetTesting"
if not exist "%destination%" mkdir "%destination%"

for /f %%F in ('call dir *.nupkg /b') do (
	echo Copying the file: %%F
    copy "%%F" "%destination%"
)

echo.