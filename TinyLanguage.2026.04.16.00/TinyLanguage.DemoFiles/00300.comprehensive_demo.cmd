@echo off
echo Running 00300.comprehensive_demo.tlg
if "%2"=="" (
    TinyLanguage.exe 00300.comprehensive_demo.tlg output.txt
) else (
    TinyLanguage.exe 00300.comprehensive_demo.tlg %2
)
