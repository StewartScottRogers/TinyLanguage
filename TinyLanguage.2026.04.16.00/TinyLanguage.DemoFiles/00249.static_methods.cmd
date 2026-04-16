@echo off
echo Running 00249.static_methods.tlg
if "%2"=="" (
    TinyLanguage.exe 00249.static_methods.tlg output.txt
) else (
    TinyLanguage.exe 00249.static_methods.tlg %2
)
