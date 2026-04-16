@echo off
echo Running 00019.foreach_array.tlg
if "%2"=="" (
    TinyLanguage.exe 00019.foreach_array.tlg output.txt
) else (
    TinyLanguage.exe 00019.foreach_array.tlg %2
)
