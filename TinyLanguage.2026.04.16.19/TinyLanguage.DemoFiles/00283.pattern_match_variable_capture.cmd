@echo off
echo Running 00283.pattern_match_variable_capture.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000283.pattern_match_variable_capture.tlg output.txt
) else (
    TinyLanguage.exe %~dp000283.pattern_match_variable_capture.tlg %2
)
