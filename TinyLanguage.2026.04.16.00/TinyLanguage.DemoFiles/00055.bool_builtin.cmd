@echo off
echo Running 00055.bool_builtin.tlg
if "%2"=="" (
    TinyLanguage.exe 00055.bool_builtin.tlg output.txt
) else (
    TinyLanguage.exe 00055.bool_builtin.tlg %2
)
