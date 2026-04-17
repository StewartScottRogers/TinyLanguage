@echo off
echo Running 00073.while_sum.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000073.while_sum.tlg output.txt
) else (
    TinyLanguage.exe %~dp000073.while_sum.tlg %2
)
