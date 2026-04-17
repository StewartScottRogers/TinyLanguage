@echo off
echo Running 00244.gcd_lcm.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000244.gcd_lcm.tlg output.txt
) else (
    TinyLanguage.exe %~dp000244.gcd_lcm.tlg %2
)
