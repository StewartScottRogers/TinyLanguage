@echo off
echo Running 00039.compare_boolean_result.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000039.compare_boolean_result.tlg output.txt
) else (
    TinyLanguage.exe %~dp000039.compare_boolean_result.tlg %2
)
