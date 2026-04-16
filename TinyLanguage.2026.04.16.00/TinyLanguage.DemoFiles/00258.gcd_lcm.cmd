@echo off
echo Running 00258.gcd_lcm.tlg
if "%2"=="" (
    TinyLanguage.exe 00258.gcd_lcm.tlg output.txt
) else (
    TinyLanguage.exe 00258.gcd_lcm.tlg %2
)
