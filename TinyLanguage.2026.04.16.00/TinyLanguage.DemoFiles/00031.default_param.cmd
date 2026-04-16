@echo off
echo Running 00031.default_param.tlg
if "%2"=="" (
    TinyLanguage.exe 00031.default_param.tlg output.txt
) else (
    TinyLanguage.exe 00031.default_param.tlg %2
)
