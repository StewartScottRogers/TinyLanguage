@echo off
echo Running 00091.foreach_array.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000091.foreach_array.tlg output.txt
) else (
    TinyLanguage.exe %~dp000091.foreach_array.tlg %2
)
