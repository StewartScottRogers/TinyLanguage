@echo off
echo Running 00305.class_method_chain.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000305.class_method_chain.tlg output.txt
) else (
    TinyLanguage.exe %~dp000305.class_method_chain.tlg %2
)
