@echo off
echo Running 00053.str_builtin.tlg
if "%2"=="" (
    TinyLanguage.exe 00053.str_builtin.tlg output.txt
) else (
    TinyLanguage.exe 00053.str_builtin.tlg %2
)
