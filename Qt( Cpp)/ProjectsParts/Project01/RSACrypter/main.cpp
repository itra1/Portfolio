#include <QCoreApplication>
#include <QDir>
#include <QDebug>
#include "../RSAStaticLib/rsastaticlib.h"

//void EncodeFile(QString source, QString target){

//  //QString sourcePath = QDir::currentPath()+"/"+source;
//  //QString targetPath = QDir::currentPath()+"/"+target;
//  QString sourcePath = source;
//  QString targetPath = target;

//  qDebug() << "Start encrupt";
//  qDebug() << "Source file " << sourcePath;
//  qDebug() << "Target file " << targetPath;

//  QFile sFile(sourcePath);
//  sFile.open(QFile::ReadOnly);
//  QByteArray ab = sFile.readAll();
//  sFile.close();

//  RSAStaticLib *lib = new RSAStaticLib();
//  auto enc = lib->encruptToBase64(ab);

//  QFile tFile(targetPath);
//  tFile.open(QFile::WriteOnly);
//  //tFile.write(b.toBase64());
//  tFile.write(QByteArray::fromStdString(enc));
//  tFile.close();

//}


//void DecodeFile(QString source, QString target){

//  //QString sourcePath = QDir::currentPath()+"/"+source;
//  //QString targetPath = QDir::currentPath()+"/"+target;
//  QString sourcePath = source;
//  QString targetPath = target;

//  qDebug() << "Start decrupt";
//  qDebug() << "Source file " << sourcePath;
//  qDebug() << "Target file " << targetPath;

//  QFile sFile(sourcePath);
//  sFile.open(QFile::ReadOnly);
//  QByteArray ab = sFile.readAll();
//  sFile.close();

//  RSAStaticLib *lib = new RSAStaticLib();
//  auto decode = lib->decruptFromBase64(ab);

//  QFile tFile(targetPath);
//  tFile.open(QFile::WriteOnly);
//  tFile.write(QByteArray::fromStdString(decode));
//  tFile.close();
//}

int main(int argc, char *argv[])
{
  QCoreApplication a(argc, argv);

  bool isDecrupt = false;
  QString _sourcePath = "";
  QString _targetPath = "";

  for(int i = 0 ; i < argc ; i++){
      QString key = argv[i];

      if(key == "decrupt")
        isDecrupt = true;

      if(key == "source"){
          _sourcePath = argv[++i];
        }
      if(key == "target"){
          _targetPath = argv[++i];
        }
    }
//    if(isDecrupt){
//        DecodeFile(_sourcePath,_targetPath);
//      }else{
//        EncodeFile(_sourcePath,_targetPath);
//      }
  return 1;
}
