@echo off
setlocal
cd /d "%~dp0"

rem Torch unpacks files whose names exceed the 260-char Windows path limit
rem inside this nested repo. Keep the real venv on a short path.
set "VENV_DIR=C:\nv-ml-venv"

if exist ".venv" (
  rmdir .venv 2>nul
  if exist ".venv" (
    echo Removing the old long-path .venv, this can take a minute...
    rmdir /s /q .venv
  )
)

if not exist "%VENV_DIR%\Scripts\python.exe" (
  python -m venv "%VENV_DIR%"
)

mklink /J .venv "%VENV_DIR%" >nul 2>&1

echo Installing packages into %VENV_DIR% ...
"%VENV_DIR%\Scripts\python.exe" -m pip install --upgrade pip
"%VENV_DIR%\Scripts\python.exe" -m pip install -r requirements.txt
if errorlevel 1 (
  echo.
  echo Install failed. If you still see a long-path error, run Command Prompt as Administrator:
  echo   reg add HKLM\SYSTEM\CurrentControlSet\Control\FileSystem /v LongPathsEnabled /t REG_DWORD /d 1 /f
  echo then reboot and run setup.cmd again.
  exit /b 1
)
where nvidia-smi >nul 2>&1
if not errorlevel 1 (
  echo Installing CUDA PyTorch for GPU training...
  "%VENV_DIR%\Scripts\python.exe" -m pip install torch torchvision --index-url https://download.pytorch.org/whl/cu128
)

echo.
echo Ready. Use:
echo   C:\nv-ml-venv\Scripts\python.exe TumorDetection.ML.py -h
echo   C:\nv-ml-venv\Scripts\python.exe TumorDetection.ML.py download
echo   C:\nv-ml-venv\Scripts\python.exe TumorDetection.ML.py train --task all --smoke
endlocal
