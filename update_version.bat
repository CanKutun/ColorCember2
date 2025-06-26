
@echo off

REM GitHub projesinin yerel klasör yolunu buraya yaz
cd C:\Users\canku\COLREKODULLUYEDEK

REM Main branch'e geç (detached HEAD durumunu düzeltir)
git checkout main

REM Uzak depo ile yeniden senkronize ol
git pull --rebase origin main

REM version.txt dosyasına en son sürümü yaz
echo 2.3.1 > version.txt

REM Değişiklikleri git’e ekle
git add version.txt

REM Commit mesajı ile sürüm güncellemesini kaydet
git commit -m "Versiyon 2.3.1 olarak guncellendi"

REM Değişiklikleri GitHub'a gönder
git push origin main

pause
