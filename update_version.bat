@echo off

REM GitHub projesinin yerel klasör yolunu buraya yaz
cd /d C:\Users\canku\COLREKODULLUYEDEK

REM version.txt dosyasına en son sürümü yaz (buraya yeni sürümü yazabilirsin)
echo 2.2.9 > version.txt

REM Değişiklikleri git’e ekle
git add version.txt

REM Commit mesajı ile sürüm güncellemesini kaydet
git commit -m "Versiyon 2.2.9 olarak guncellendi"

REM Değişiklikleri GitHub'a gönder
git push origin main

pause
