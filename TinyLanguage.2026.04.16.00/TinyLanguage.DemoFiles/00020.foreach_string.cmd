@echo off
echo Running 00020.foreach_string.tlg
if "%2"=="" (
    TinyLanguage.exe 00020.foreach_string.tlg output.txt
) else (
    TinyLanguage.exe 00020.foreach_string.tlg %2
)
