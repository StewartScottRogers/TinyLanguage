@echo off
echo Running 00212.class_field_annotations.tlg
if "%2"=="" (
    TinyLanguage.exe 00212.class_field_annotations.tlg output.txt
) else (
    TinyLanguage.exe 00212.class_field_annotations.tlg %2
)
