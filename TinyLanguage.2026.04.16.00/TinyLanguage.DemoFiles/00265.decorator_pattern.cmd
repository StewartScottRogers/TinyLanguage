@echo off
echo Running 00265.decorator_pattern.tlg
if "%2"=="" (
    TinyLanguage.exe 00265.decorator_pattern.tlg output.txt
) else (
    TinyLanguage.exe 00265.decorator_pattern.tlg %2
)
