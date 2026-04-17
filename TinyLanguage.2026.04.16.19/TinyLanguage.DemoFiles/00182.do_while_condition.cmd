@echo off
echo Running 00182.do_while_condition.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000182.do_while_condition.tlg output.txt
) else (
    TinyLanguage.exe %~dp000182.do_while_condition.tlg %2
)
