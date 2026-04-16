@echo off
echo Running 00235.interface_style.tlg
if "%2"=="" (
    TinyLanguage.exe 00235.interface_style.tlg output.txt
) else (
    TinyLanguage.exe 00235.interface_style.tlg %2
)
