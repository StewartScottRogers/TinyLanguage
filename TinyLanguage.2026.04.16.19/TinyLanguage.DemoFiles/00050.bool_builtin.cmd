@echo off
echo Running 00050.bool_builtin.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000050.bool_builtin.tlg output.txt
) else (
    TinyLanguage.exe %~dp000050.bool_builtin.tlg %2
)
