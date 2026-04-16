@echo off
echo Running 00234.deep_inheritance.tlg
if "%2"=="" (
    TinyLanguage.exe 00234.deep_inheritance.tlg output.txt
) else (
    TinyLanguage.exe 00234.deep_inheritance.tlg %2
)
