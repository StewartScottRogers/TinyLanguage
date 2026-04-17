@echo off
echo Running 00102.do_while_once.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000102.do_while_once.tlg output.txt
) else (
    TinyLanguage.exe %~dp000102.do_while_once.tlg %2
)
