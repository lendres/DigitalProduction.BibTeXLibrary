@echo off
title NuGet Publishing
setlocal enabledelayedexpansion

rem Get the api key.
echo.
set /p api-key=<"../../../../Keys/Nuget/api-key.txt"
echo API Key: %api-key%

rem Change to the package directory.
chdir /d ../bin/Release
echo.
echo Processing the directory: %cd%

rem Find only the latest package version.
set "highest_major=0"
set "highest_minor=0"
set "highest_build=0"
set "best_file="

for /f %%F in ('call dir *.nupkg /b') do (
	echo Reviewing the file: %%F
    set "filename=%%~nF"
    
    for /f "tokens=1-4 delims=." %%A in ("!filename!") do (
        set "basename=%%A"
        set "major=%%B"
        set "minor=%%C"
        set "build=%%D"
        
        rem Check if the current file has a higher version
        if !major! gtr !highest_major! (
            set "highest_major=!major!"
            set "highest_minor=!minor!"
            set "highest_build=!build!"
            set "best_file=%%F"
        ) else if !major! equ !highest_major! (
            if !minor! gtr !highest_minor! (
                set "highest_minor=!minor!"
                set "highest_build=!build!"
                set "best_file=%%F"
            ) else if !minor! equ !highest_minor! (
                if !build! gtr !highest_build! (
                    set "highest_build=!build!"
                    set "best_file=%%F"
                )
            )
        )
    )
)

rem Publish.
echo.
echo Publishing: %best_file%
dotnet nuget push %best_file% --api-key %api-key% --source https://api.nuget.org/v3/index.json --skip-duplicate

echo.
pause