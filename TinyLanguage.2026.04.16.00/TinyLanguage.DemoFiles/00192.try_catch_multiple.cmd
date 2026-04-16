@echo off
echo Running 00192.try_catch_multiple.tlg
if "%2"=="" (
    TinyLanguage.exe 00192.try_catch_multiple.tlg output.txt
) else (
    TinyLanguage.exe 00192.try_catch_multiple.tlg %2
)
