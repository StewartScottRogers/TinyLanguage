@echo off
echo Running 00043.compound_assign_add.tlg
if "%2"=="" (
    TinyLanguage.exe 00043.compound_assign_add.tlg output.txt
) else (
    TinyLanguage.exe 00043.compound_assign_add.tlg %2
)
