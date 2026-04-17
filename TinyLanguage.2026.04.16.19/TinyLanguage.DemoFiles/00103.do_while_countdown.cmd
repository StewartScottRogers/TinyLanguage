@echo off
echo Running 00103.do_while_countdown.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000103.do_while_countdown.tlg output.txt
) else (
    TinyLanguage.exe %~dp000103.do_while_countdown.tlg %2
)
