@echo off
echo Running 00222.accumulator_advanced.tlg
if "%2"=="" (
    TinyLanguage.exe 00222.accumulator_advanced.tlg output.txt
) else (
    TinyLanguage.exe 00222.accumulator_advanced.tlg %2
)
