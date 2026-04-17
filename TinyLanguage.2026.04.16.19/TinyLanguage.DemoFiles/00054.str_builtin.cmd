@echo off
echo Running 00054.str_builtin.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000054.str_builtin.tlg output.txt
) else (
    TinyLanguage.exe %~dp000054.str_builtin.tlg %2
)
