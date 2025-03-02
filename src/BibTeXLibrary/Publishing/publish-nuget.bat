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
	rem Get the file name minus extension.
    set "filename=%%~nF"
    
    rem Split the filename into parts using dot as delimiter.
    set "parts=!filename:.= !"
    set "major=0"
    set "minor=0"
    set "build=0"
	set "count=0"
    
    rem Count total parts.
    for %%A in (!parts!) do set /a "count+=1"
    
    rem Extract the last 3 parts corresponding to the MAJOR.MINOR.BUILD. parts.
    set "i=0"
    for %%A in (!parts!) do (
        set /a "i+=1"
		set /a "t=count-2"
        if !i! equ !t! (set "major=%%A")
		set /a "t=count-1"
        if !i! equ !t! (set "minor=%%A")
		set /a "t=count"
        if !i! equ !t! (set "build=%%A")
    )

    rem echo Highest !highest_major!.!highest_minor!.!highest_build!
	rem echo Current !major!.!minor!.!build!

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

rem Publish.
echo.
echo Publishing: %best_file%
dotnet nuget push %best_file% --api-key %api-key% --source https://api.nuget.org/v3/index.json --skip-duplicate

echo.
pause