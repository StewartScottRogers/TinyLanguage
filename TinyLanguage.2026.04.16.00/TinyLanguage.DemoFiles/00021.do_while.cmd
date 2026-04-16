@echo off
echo Running 00021.do_while.tlg
if "%2"=="" (
    TinyLanguage.exe 00021.do_while.tlg output.txt
) else (
    TinyLanguage.exe 00021.do_while.tlg %2
)
