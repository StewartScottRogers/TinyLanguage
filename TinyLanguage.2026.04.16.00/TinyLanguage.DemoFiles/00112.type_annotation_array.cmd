@echo off
echo Running 00112.type_annotation_array.tlg
if "%2"=="" (
    TinyLanguage.exe 00112.type_annotation_array.tlg output.txt
) else (
    TinyLanguage.exe 00112.type_annotation_array.tlg %2
)
