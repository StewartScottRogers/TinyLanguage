@echo off
echo Running 00320.final_showcase.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000320.final_showcase.tlg output.txt
) else (
    TinyLanguage.exe %~dp000320.final_showcase.tlg %2
)
