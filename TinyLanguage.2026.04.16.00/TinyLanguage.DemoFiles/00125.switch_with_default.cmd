@echo off
echo Running 00125.switch_with_default.tlg
if "%2"=="" (
    TinyLanguage.exe 00125.switch_with_default.tlg output.txt
) else (
    TinyLanguage.exe 00125.switch_with_default.tlg %2
)
