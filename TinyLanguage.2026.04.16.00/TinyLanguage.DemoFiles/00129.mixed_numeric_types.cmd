@echo off
echo Running 00129.mixed_numeric_types.tlg
if "%2"=="" (
    TinyLanguage.exe 00129.mixed_numeric_types.tlg output.txt
) else (
    TinyLanguage.exe 00129.mixed_numeric_types.tlg %2
)
