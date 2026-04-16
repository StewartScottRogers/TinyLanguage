@echo off
echo Running 00044.compound_assign_sub.tlg
if "%2"=="" (
    TinyLanguage.exe 00044.compound_assign_sub.tlg output.txt
) else (
    TinyLanguage.exe 00044.compound_assign_sub.tlg %2
)
