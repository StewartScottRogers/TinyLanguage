@echo off
echo Running 00271.mixin_composition.tlg
if "%2"=="" (
    TinyLanguage.exe 00271.mixin_composition.tlg output.txt
) else (
    TinyLanguage.exe 00271.mixin_composition.tlg %2
)
