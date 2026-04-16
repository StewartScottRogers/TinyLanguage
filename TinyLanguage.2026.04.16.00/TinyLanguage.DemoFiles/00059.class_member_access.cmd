@echo off
echo Running 00059.class_member_access.tlg
if "%2"=="" (
    TinyLanguage.exe 00059.class_member_access.tlg output.txt
) else (
    TinyLanguage.exe 00059.class_member_access.tlg %2
)
