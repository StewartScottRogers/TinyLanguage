@echo off
echo Running 00077.while_false_condition.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000077.while_false_condition.tlg output.txt
) else (
    TinyLanguage.exe %~dp000077.while_false_condition.tlg %2
)
