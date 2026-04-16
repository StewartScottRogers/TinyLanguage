@echo off
echo Running 00046.compound_assign_div.tlg
if "%2"=="" (
    TinyLanguage.exe 00046.compound_assign_div.tlg output.txt
) else (
    TinyLanguage.exe 00046.compound_assign_div.tlg %2
)
