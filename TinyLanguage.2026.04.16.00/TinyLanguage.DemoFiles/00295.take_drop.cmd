@echo off
echo Running 00295.take_drop.tlg
if "%2"=="" (
    TinyLanguage.exe 00295.take_drop.tlg output.txt
) else (
    TinyLanguage.exe 00295.take_drop.tlg %2
)
