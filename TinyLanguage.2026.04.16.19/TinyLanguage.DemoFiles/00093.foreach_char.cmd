@echo off
echo Running 00093.foreach_char.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000093.foreach_char.tlg output.txt
) else (
    TinyLanguage.exe %~dp000093.foreach_char.tlg %2
)
