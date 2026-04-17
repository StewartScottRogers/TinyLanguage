@echo off
echo Running 00116.function_default_param.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000116.function_default_param.tlg output.txt
) else (
    TinyLanguage.exe %~dp000116.function_default_param.tlg %2
)
