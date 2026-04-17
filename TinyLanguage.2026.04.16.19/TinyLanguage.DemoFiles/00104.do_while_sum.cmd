@echo off
echo Running 00104.do_while_sum.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000104.do_while_sum.tlg output.txt
) else (
    TinyLanguage.exe %~dp000104.do_while_sum.tlg %2
)
