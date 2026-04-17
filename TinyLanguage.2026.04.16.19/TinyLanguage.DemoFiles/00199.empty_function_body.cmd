@echo off
echo Running 00199.empty_function_body.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000199.empty_function_body.tlg output.txt
) else (
    TinyLanguage.exe %~dp000199.empty_function_body.tlg %2
)
