@echo off
echo Running 00017.augmented_multiply.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000017.augmented_multiply.tlg output.txt
) else (
    TinyLanguage.exe %~dp000017.augmented_multiply.tlg %2
)
