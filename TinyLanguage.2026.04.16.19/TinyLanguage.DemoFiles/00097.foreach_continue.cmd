@echo off
echo Running 00097.foreach_continue.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000097.foreach_continue.tlg output.txt
) else (
    TinyLanguage.exe %~dp000097.foreach_continue.tlg %2
)
