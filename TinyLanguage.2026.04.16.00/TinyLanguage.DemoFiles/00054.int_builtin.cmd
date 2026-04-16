@echo off
echo Running 00054.int_builtin.tlg
if "%2"=="" (
    TinyLanguage.exe 00054.int_builtin.tlg output.txt
) else (
    TinyLanguage.exe 00054.int_builtin.tlg %2
)
