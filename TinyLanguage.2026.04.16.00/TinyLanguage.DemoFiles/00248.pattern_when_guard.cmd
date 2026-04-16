@echo off
echo Running 00248.pattern_when_guard.tlg
if "%2"=="" (
    TinyLanguage.exe 00248.pattern_when_guard.tlg output.txt
) else (
    TinyLanguage.exe 00248.pattern_when_guard.tlg %2
)
