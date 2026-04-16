@echo off
echo Running 00247.tail_recursion_style.tlg
if "%2"=="" (
    TinyLanguage.exe 00247.tail_recursion_style.tlg output.txt
) else (
    TinyLanguage.exe 00247.tail_recursion_style.tlg %2
)
