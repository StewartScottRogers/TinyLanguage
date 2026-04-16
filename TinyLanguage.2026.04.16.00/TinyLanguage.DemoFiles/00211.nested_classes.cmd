@echo off
echo Running 00211.nested_classes.tlg
if "%2"=="" (
    TinyLanguage.exe 00211.nested_classes.tlg output.txt
) else (
    TinyLanguage.exe 00211.nested_classes.tlg %2
)
