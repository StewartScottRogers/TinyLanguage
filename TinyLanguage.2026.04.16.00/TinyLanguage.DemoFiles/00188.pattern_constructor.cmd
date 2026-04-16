@echo off
echo Running 00188.pattern_constructor.tlg
if "%2"=="" (
    TinyLanguage.exe 00188.pattern_constructor.tlg output.txt
) else (
    TinyLanguage.exe 00188.pattern_constructor.tlg %2
)
