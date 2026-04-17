@echo off
echo Running 00063.if_elif_else.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000063.if_elif_else.tlg output.txt
) else (
    TinyLanguage.exe %~dp000063.if_elif_else.tlg %2
)
