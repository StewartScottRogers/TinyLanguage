@echo off
echo Running 00145.class_this.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000145.class_this.tlg output.txt
) else (
    TinyLanguage.exe %~dp000145.class_this.tlg %2
)
