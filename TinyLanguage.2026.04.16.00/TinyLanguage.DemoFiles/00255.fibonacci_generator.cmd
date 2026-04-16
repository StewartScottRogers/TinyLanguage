@echo off
echo Running 00255.fibonacci_generator.tlg
if "%2"=="" (
    TinyLanguage.exe 00255.fibonacci_generator.tlg output.txt
) else (
    TinyLanguage.exe 00255.fibonacci_generator.tlg %2
)
