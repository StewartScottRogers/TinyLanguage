@echo off
echo Running 00266.observer_pattern.tlg
if "%2"=="" (
    TinyLanguage.exe 00266.observer_pattern.tlg output.txt
) else (
    TinyLanguage.exe 00266.observer_pattern.tlg %2
)
