@echo off
echo Running 00139.accumulator_pattern.tlg
if "%2"=="" (
    TinyLanguage.exe 00139.accumulator_pattern.tlg output.txt
) else (
    TinyLanguage.exe 00139.accumulator_pattern.tlg %2
)
