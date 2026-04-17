@echo off
echo Running 00101.do_while_simple.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000101.do_while_simple.tlg output.txt
) else (
    TinyLanguage.exe %~dp000101.do_while_simple.tlg %2
)
