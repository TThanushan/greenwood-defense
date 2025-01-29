@echo off
echo Press any key to confirm deletion...
pause > nul
echo Press any key again to confirm...
pause > nul

echo Deleting directories...

if exist "Assets" (
    rmdir /s /q "Assets"
    echo .
) else (
    echo .
)

if exist "ProjectSettings" (
    rmdir /s /q "ProjectSettings"
    echo .
) else (
    echo .
)

if exist "Packages" (
    rmdir /s /q "Packages"
    echo .
) else (
    echo .
)
