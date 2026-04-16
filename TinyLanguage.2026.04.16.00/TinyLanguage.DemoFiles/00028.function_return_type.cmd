@echo off
echo Running 00028.function_return_type.tlg
if "%2"=="" (
    TinyLanguage.exe 00028.function_return_type.tlg output.txt
) else (
    TinyLanguage.exe 00028.function_return_type.tlg %2
)
