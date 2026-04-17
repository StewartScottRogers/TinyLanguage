@echo off
echo Running 00096.foreach_break.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000096.foreach_break.tlg output.txt
) else (
    TinyLanguage.exe %~dp000096.foreach_break.tlg %2
)
