@echo off
echo Running 00211.additive_vs_multiplicative.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000211.additive_vs_multiplicative.tlg output.txt
) else (
    TinyLanguage.exe %~dp000211.additive_vs_multiplicative.tlg %2
)
