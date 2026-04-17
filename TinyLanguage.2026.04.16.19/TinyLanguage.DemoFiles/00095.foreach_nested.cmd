@echo off
echo Running 00095.foreach_nested.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000095.foreach_nested.tlg output.txt
) else (
    TinyLanguage.exe %~dp000095.foreach_nested.tlg %2
)
