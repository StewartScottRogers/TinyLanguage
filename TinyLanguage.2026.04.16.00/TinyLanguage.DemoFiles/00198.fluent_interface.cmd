@echo off
echo Running 00198.fluent_interface.tlg
if "%2"=="" (
    TinyLanguage.exe 00198.fluent_interface.tlg output.txt
) else (
    TinyLanguage.exe 00198.fluent_interface.tlg %2
)
