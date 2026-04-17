@echo off
echo Running 00288.class_inheritance_poly.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000288.class_inheritance_poly.tlg output.txt
) else (
    TinyLanguage.exe %~dp000288.class_inheritance_poly.tlg %2
)
