@echo off
echo Running 00048.truthiness_null.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000048.truthiness_null.tlg output.txt
) else (
    TinyLanguage.exe %~dp000048.truthiness_null.tlg %2
)
