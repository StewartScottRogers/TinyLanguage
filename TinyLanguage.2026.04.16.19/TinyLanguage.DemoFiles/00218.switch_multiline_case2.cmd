@echo off
echo Running 00218.switch_multiline_case2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000218.switch_multiline_case2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000218.switch_multiline_case2.tlg %2
)
