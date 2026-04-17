@echo off
echo Running 00016.augmented_subtract.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000016.augmented_subtract.tlg output.txt
) else (
    TinyLanguage.exe %~dp000016.augmented_subtract.tlg %2
)
