@echo off
echo Running 00092.foreach_strings.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000092.foreach_strings.tlg output.txt
) else (
    TinyLanguage.exe %~dp000092.foreach_strings.tlg %2
)
