@echo off
echo Running 00163.string_ltrim.tlg
if "%2"=="" (
    TinyLanguage.exe 00163.string_ltrim.tlg output.txt
) else (
    TinyLanguage.exe 00163.string_ltrim.tlg %2
)
