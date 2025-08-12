#-*- coding: UTF-8 -*-
#
#python.exe %{sourceDir}\collecter.py %{ActiveProject:Name} %{ActiveProject:BuildConfig:Path} "d:/Work/CNP/Launcher/Archivate" "g:/work/CNP/builds"
from sys import argv
import os
import shutil
from os import listdir
from os.path import isfile, join, isdir
import zipfile
from win32api import *

def addFileToArchiveRecursive(fileZip,archivePathFolder,archiveFolder,targetFolder):
  #print(archiveFolder)
  targetFolder = archivePathFolder + targetFolder
  #print(targetFolder)
  onlyfiles = [f for f in listdir(targetFolder) if isfile(join(targetFolder, f))]
  for value in onlyfiles:
    #print(F"{targetFolder}{value}" )
    fileZip.write( F"{targetFolder}{value}" , F"{archiveFolder}{value}", compress_type=zipfile.ZIP_DEFLATED)
  onlyDir = [f for f in listdir(targetFolder) if isdir(join(targetFolder, f))]
  for folder in onlyDir:
    #print(folder + "/")
    addFileToArchiveRecursive(fileZip,targetFolder,archiveFolder + folder + "/",folder + "/")

script, projectName, buildPath, dirArchive, allBuilds = argv

dirArchive = dirArchive.replace('"', '')
archivePath = dirArchive                                        # Чистка строка
archivePathExe = F"{archivePath}/{projectName}.exe"               # Целевой файл копирования
archivePathFolder = archivePath + "/Archive"                    # Котолог который архивируется
archivePathFolderExe = F"{archivePathFolder}/{projectName}.exe"   # Дубликат целевого копирования
buildPathExe = F"{buildPath}/{projectName}.exe"

print(F"GetFileVersionInfo {buildPathExe}")
File_information = GetFileVersionInfo(buildPathExe, "\\")
print(File_information)
ms_file_version = File_information['FileVersionMS']
ls_file_version = File_information['FileVersionLS']
v = [str(HIWORD(ms_file_version)), str(LOWORD(ms_file_version)), str(HIWORD(ls_file_version)), str(LOWORD(ls_file_version))]
version = ".".join(v)
print(F"Version {version}")

zipFileName = F"/Launcher_{version}.zip"
zipFilePath = dirArchive + F"/{zipFileName}"  # Целевой архив
allBuildArray = dirArchive + "/../../Builds"

# Копирование EXE

print("Source app ", buildPathExe)
print("Archive path ", dirArchive)
shutil.copy(buildPathExe, archivePathExe)
shutil.copy(buildPathExe, archivePathFolderExe)

print(F"Copy {buildPathExe} to {archivePath}")

# Архивирование
print(F"Archivation {zipFilePath}")

jungle_zip = zipfile.ZipFile(zipFilePath, 'w')
addFileToArchiveRecursive(jungle_zip,archivePathFolder,"/","/")
jungle_zip.close()

print(F"Archivation complete")

# Перемещяем в общий каталог сборки
print(F"Copy archive to " + (allBuilds + zipFileName))
shutil.copy(zipFilePath, F"{allBuilds}/{zipFileName}")

print(F"Complete collecter")