@echo off
echo Running 00189.pattern_array.tlg
if "%2"=="" (
    TinyLanguage.exe 00189.pattern_array.tlg output.txt
) else (
    TinyLanguage.exe 00189.pattern_array.tlg %2
)
