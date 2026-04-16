@echo off
echo Running 00116.static_class.tlg
if "%2"=="" (
    TinyLanguage.exe 00116.static_class.tlg output.txt
) else (
    TinyLanguage.exe 00116.static_class.tlg %2
)
