@echo off
echo Running 00191.pattern_match_when_guard.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000191.pattern_match_when_guard.tlg output.txt
) else (
    TinyLanguage.exe %~dp000191.pattern_match_when_guard.tlg %2
)
