#include "iohelper.h"
#include <QDebug>
#include <QDir>
#include <QFile>

namespace QtSystemLib {
using namespace std;

char buf[BUFSIZ];
size_t size;

void IOHelper::CopyFile(QString sourceFile, QString targetFile)
{
  if(QFile::exists(targetFile))
    QFile::remove(targetFile);
  QFile::copy(sourceFile, targetFile);
}
void IOHelper::MoveFile(QString sourceFile, QString targetFile)
{
	if (QFile::exists(targetFile))
		QFile::remove(targetFile);
    QFile::rename(sourceFile, targetFile);
}

void IOHelper::CopyPath(QString sourcePath, QString targetPath)
{
  QDir source(sourcePath);
  QDir target(targetPath);

  if(!target.exists(targetPath))
    target.mkdir(targetPath);

  QStringList dirsList = source.entryList(QDir::Dirs);

  for(int i = 0 ;i < dirsList.count();i++){
    QString val = dirsList.value(i);
    if(val == "." || val == "..") continue;
    QString resPath(targetPath + "/" + val);
    QDir resDir(resPath);
    resDir.mkdir(resPath);
    qDebug() << "Create path " << resPath;
    CopyPath(sourcePath + "/" + val, targetPath + "/" + val);
  }

  QStringList filesList = source.entryList(QDir::Files);

  for(int i = 0 ;i < filesList.count();i++){
    QString val = filesList.value(i);
    QString resPath(targetPath + "/" + val);
    CopyFile(sourcePath + "/" + val, targetPath + "/" + val);
  }
}

QByteArray IOHelper::TextFileRead(QString path)
{
  QFile file(path);
  if (!file.exists()) {
    qDebug() << "No exists file " << path;
    return nullptr;
  }
  if (!file.open(QFile::ReadOnly | QFile::Text)) {
    qDebug() << "Error read file " << path;
    return nullptr;
  }
  auto data = file.readAll();
  file.close();
  return data;
}
} // namespace QtSystemLib
