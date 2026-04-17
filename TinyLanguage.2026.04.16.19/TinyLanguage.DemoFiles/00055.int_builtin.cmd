@echo off
echo Running 00055.int_builtin.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000055.int_builtin.tlg output.txt
) else (
    TinyLanguage.exe %~dp000055.int_builtin.tlg %2
)
