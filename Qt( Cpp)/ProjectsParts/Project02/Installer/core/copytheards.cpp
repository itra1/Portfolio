#include "copytheards.h"
#include <QDir>
#include <QFile>
#include <QDebug>
#include <QThread>

#include "math.h"

CopyTheards::CopyTheards(QObject *parent)
    : QObject(parent)
    , m_listFiles(new QList<QString>)
    , m_listDirs(new QList<QString>)
{
}

namespace  {

    double recursiveRead(QFileInfo fileInfo, QList<QString> *listFiles, QList<QString> *listDirs){

        double size = 0;

        QFileInfoList fileInfoList = QDir(fileInfo.path() + "/" + fileInfo.fileName()).entryInfoList(QDir::Dirs | QDir::Files);

        for(QFileInfo fi : fileInfoList){

            if(fi.fileName() == "." || fi.fileName() == "..")
                continue;

            if(fi.isFile()){
                size += fi.size();
                listFiles->append(fi.path() + "/" + fi.fileName());
            }
            if(fi.isDir()){
                listDirs->append(fi.path() + "/" + fi.fileName());
                size += recursiveRead(QFileInfo(fi.path() + "/" + fi.fileName()),listFiles,listDirs);
            }
        }

        return size;
    }
}

void CopyTheards::run()
{
    emit onStart();

    m_listFiles->clear();
    m_listDirs->clear();

    QString startfolder = m_fromPath;
    QString destfolder = m_toPath;
    qDebug() << "Start folder " + startfolder;
    qDebug() << "destfolder folder " + destfolder;


    QDir dd(destfolder);

    if(!dd.exists())
        dd.mkpath(dd.path());

    m_sizeFiles = recursiveRead(QFileInfo(startfolder),m_listFiles,m_listDirs);

    for(QString fi : *m_listDirs){

        QDir dir(destfolder + fi.remove(0,startfolder.length()));
        if(!dir.exists())
            dir.mkpath(dir.path());
    }

    emit onStartCopy();

    sizeCopy = 0;
    emit onProgress(0);

    for(QString fi : *m_listFiles){
        QFile sourceFile(fi);

        QString targetPath = destfolder + fi.remove(0,startfolder.length());

        QFile destFile(targetPath);
        //QFile destFile(targetPath + fi.remove(0,startfolder.length()));

        sourceFile.open(QIODevice::ReadOnly);
        destFile.open(QIODevice::WriteOnly);

        qint64 fileSize = sourceFile.size();

        for (qint64 i = 0, ii = 0; i <= fileSize;  )
        {
            destFile.write(sourceFile.read(i));
            sourceFile.seek(i);
            destFile.seek(i);

            sizeCopy += i - ii;
            emit onProgress(double(sizeCopy) / m_sizeFiles);

            ii = i;

            if(i != fileSize){
                i += fmin(fileSize-i,10485760);
            }else{
                i++;
            }
        }

        sourceFile.close();
        destFile.close();
    }
    emit onProgress(1);

//    if(QDir(startfolder).exists())
//        QDir(startfolder).removeRecursively();

    emit onFinished();
}

void CopyTheards::setToPath(const QString &toPath)
{
    m_toPath = toPath;
}

void CopyTheards::setFromPath(const QString &fromPath)
{
    m_fromPath = fromPath;
}


