@echo off
echo Running 00079.is_type_check.tlg
if "%2"=="" (
    TinyLanguage.exe 00079.is_type_check.tlg output.txt
) else (
    TinyLanguage.exe 00079.is_type_check.tlg %2
)
