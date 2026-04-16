@echo off
echo Running 00298.multireturn_tuple.tlg
if "%2"=="" (
    TinyLanguage.exe 00298.multireturn_tuple.tlg output.txt
) else (
    TinyLanguage.exe 00298.multireturn_tuple.tlg %2
)
