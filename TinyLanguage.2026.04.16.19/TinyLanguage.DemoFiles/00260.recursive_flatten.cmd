@echo off
echo Running 00260.recursive_flatten.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000260.recursive_flatten.tlg output.txt
) else (
    TinyLanguage.exe %~dp000260.recursive_flatten.tlg %2
)
