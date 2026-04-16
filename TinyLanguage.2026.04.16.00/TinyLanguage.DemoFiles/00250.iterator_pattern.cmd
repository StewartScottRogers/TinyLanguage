@echo off
echo Running 00250.iterator_pattern.tlg
if "%2"=="" (
    TinyLanguage.exe 00250.iterator_pattern.tlg output.txt
) else (
    TinyLanguage.exe 00250.iterator_pattern.tlg %2
)
