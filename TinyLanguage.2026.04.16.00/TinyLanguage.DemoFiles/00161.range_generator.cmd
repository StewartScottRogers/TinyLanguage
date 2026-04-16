@echo off
echo Running 00161.range_generator.tlg
if "%2"=="" (
    TinyLanguage.exe 00161.range_generator.tlg output.txt
) else (
    TinyLanguage.exe 00161.range_generator.tlg %2
)
