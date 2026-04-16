@echo off
echo Running 00268.strategy_pattern.tlg
if "%2"=="" (
    TinyLanguage.exe 00268.strategy_pattern.tlg output.txt
) else (
    TinyLanguage.exe 00268.strategy_pattern.tlg %2
)
