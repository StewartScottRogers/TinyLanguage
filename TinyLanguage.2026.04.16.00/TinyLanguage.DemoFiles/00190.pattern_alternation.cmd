@echo off
echo Running 00190.pattern_alternation.tlg
if "%2"=="" (
    TinyLanguage.exe 00190.pattern_alternation.tlg output.txt
) else (
    TinyLanguage.exe 00190.pattern_alternation.tlg %2
)
