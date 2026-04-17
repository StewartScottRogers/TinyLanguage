@echo off
echo Running 00015.augmented_add.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000015.augmented_add.tlg output.txt
) else (
    TinyLanguage.exe %~dp000015.augmented_add.tlg %2
)
