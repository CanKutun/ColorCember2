@echo off
setlocal enabledelayedexpansion

REM Proje klasörüne git
cd /d C:\Users\canku\COLREKODULLUYEDEK

REM Main branch'e geç (detached HEAD durumunu düzeltir)
git checkout main

REM Uzak depo ile yeniden senkronize ol
git pull --rebase origin main

REM version.txt içinden mevcut versiyonu oku
set /p ver=<version.txt

REM Versiyonu böl (örnek: 2.3.4)
for /f "tokens=1-3 delims=." %%a in ("!ver!") do (
    set major=%%a
    set minor=%%b
    set patch=%%c
)

REM Patch versiyonunu +1 artır
set /a patch+=1

REM Yeni versiyonu oluştur
set newversion=!major!.!minor!.!patch!

REM Yeni versiyonu version.txt dosyasına yaz
echo !newversion! > version.txt

REM Değişiklikleri git’e ekle
git add version.txt

REM Commit mesajı ile güncelle
git commit -m "Versiyon !newversion! olarak guncellendi"

REM GitHub'a gönder
git push origin main

echo.
echo [✓] Yeni versiyon: !newversion!
pause
