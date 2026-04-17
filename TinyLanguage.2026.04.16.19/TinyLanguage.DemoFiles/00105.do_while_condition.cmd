@echo off
echo Running 00105.do_while_condition.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000105.do_while_condition.tlg output.txt
) else (
    TinyLanguage.exe %~dp000105.do_while_condition.tlg %2
)
