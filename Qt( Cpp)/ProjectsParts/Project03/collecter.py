#-*- coding: UTF-8 -*-
#
#%{sourceDir}\collecter.py %{ActiveProject:BuildConfig:Path} %{ActiveProject:RunConfig:Executable:FileName} %{ActiveProject:BuildConfig:Path}\..\..\..\build\%{CurrentDate:ISO}_%{CurrentTime:hh_mm_ss} %{Qt:QT_HOST_PREFIX} "i:\work\CNP\builds\"
from sys import argv
import os
import shutil
from os import listdir
from os.path import isfile, join, isdir
import zipfile
from win32api import *

def addFileToArchiveRecursive(fileZip,archivePathFolder,archiveFolder,targetFolder):
  targetFolder = archivePathFolder + targetFolder
  onlyfiles = [f for f in listdir(targetFolder) if isfile(join(targetFolder, f))]
  for value in onlyfiles:
    fileZip.write( F"{targetFolder}{value}" , F"{archiveFolder}{value}", compress_type=zipfile.ZIP_DEFLATED)
  onlyDir = [f for f in listdir(targetFolder) if isdir(join(targetFolder, f))]
  for folder in onlyDir:
    addFileToArchiveRecursive(fileZip,targetFolder,archiveFolder + folder + "/",folder + "/")

# Применение параметов
script, buildPath, exeFile, archivePath, qtPath, allBuilds = argv

allBuilds = allBuilds.replace('"', '')
buildPathExe = F"{buildPath}/{exeFile}" #Исполняемый файл
archivePathExe = F"{archivePath}/{exeFile}" #Копирование файлов

# Копирование EXE

os.makedirs(archivePath)
shutil.copy(buildPathExe, archivePathExe)
print(F"Copy {buildPathExe} to {archivePathExe}")

# Копирование OpenSSL 3
shutil.copy(F"{buildPath}\\..\\..\\toarchive\\libssl-3-x64.dll", F"{archivePath}\\libssl-3-x64.dll")
shutil.copy(F"{buildPath}\\..\\..\\toarchive\\libcrypto-3-x64.dll", F"{archivePath}\\libcrypto-3-x64.dll")

# Выполнение подборки

windeployqt = F"{qtPath}/bin/windeployqt --qmldir {qtPath}/qml {archivePathExe}"
print(F"{windeployqt}")

os.system(F"{windeployqt}")

# Архивация

File_information = GetFileVersionInfo(archivePathExe, "\\")
print(File_information)
ms_file_version = File_information['FileVersionMS']
ls_file_version = File_information['FileVersionLS']
v = [str(HIWORD(ms_file_version)), str(LOWORD(ms_file_version)), str(HIWORD(ls_file_version)), str(LOWORD(ls_file_version))]
version = ".".join(v)
print(F"Version {version}")

zipFileName = F"Browser_{version}.zip"
zipFilePath = F"{archivePath}/../{zipFileName}"  # Целевой архив
print(F"Archivation {zipFilePath}")

jungle_zip = zipfile.ZipFile(zipFilePath, 'w')
addFileToArchiveRecursive(jungle_zip,archivePath,"/","/")
jungle_zip.close()
print(F"Archivation complete")

# Перемещяем в общий каталог сборки
print(F"Copy archive to {allBuilds}\\{zipFileName}")
#shutil.copy(zipFilePath, F"{allBuilds}/{zipFileName}")
shutil.copy(F"{archivePath}\\..\\{zipFileName}", F"{allBuilds}\\{zipFileName}")

print(F"Complete collecter")