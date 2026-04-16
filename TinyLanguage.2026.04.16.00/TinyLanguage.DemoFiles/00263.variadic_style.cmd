@echo off
echo Running 00263.variadic_style.tlg
if "%2"=="" (
    TinyLanguage.exe 00263.variadic_style.tlg output.txt
) else (
    TinyLanguage.exe 00263.variadic_style.tlg %2
)
