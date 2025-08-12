#include "launcherplace.h"

#include <QDebug>
#include "sourceinfo.h"
#include "config.h"
#include "base/utils/fs.h"
#include "core/update/inieditor.h"
#include "core/logging.h"
#include <QProcess>
#include "core/launcher.h"

using namespace Core;

LauncherPlace::LauncherPlace(QObject *parent): Place (parent)
{
}

bool LauncherPlace::isGame()
{
    return false;
}

QString LauncherPlace::getSourceUrl()
{
    return m_sourceLocal != nullptr && m_sourceLocal->isDev()
         ? Config::SERVER_LAUNCHER_DEV_TORRENT
         : Config::SERVER_LAUNCHER_RELEASE_TORRENT;
}

SourceInfo* LauncherPlace::getActualVersion()
{
    SourceInfo *si = new SourceInfo(this);

    QFile sourceFile(Utils::Fs::toNativePath(QDir::currentPath() + "/source.ini"));

    if(!sourceFile.exists()){
         si->setVersion("0");
         return si;
    }

    IniEditor ini(sourceFile.fileName());

    if(si->version() == ini.read("version").toString())
        return nullptr;

    si->setVersion(ini.read("version").toString());

    si->setRunFile(ini.read("runFile").toString() );
    si->setIsInstalled(true);

    return si;
}

void LauncherPlace::checkState()
{
//    if(m_sourceLocal != nullptr && m_sourceLocal->installState() != SourceInfo::Complete)
//        return;

    SourceInfo *actualVersion = getActualVersion();

    if(m_sourceLocal != nullptr && m_sourceLocal->installState() == SourceInfo::Complete){

        if(m_sourceLocal->version() != actualVersion->version()){
            setState(PlaceState::UpdateReady);
            return;
        }
    }

    if(m_sourceServer != nullptr){
        if(m_sourceServer->version() != actualVersion->version()){
            m_sourceLocal = m_sourceServer->clone();
            m_sourceLocal->installGame();
            return;
        }
    }

    if(m_sourceServer == nullptr){
        if(m_sourceLocal != nullptr && m_sourceLocal->version() != actualVersion->version()){
            m_sourceLocal->installGame();
            return;
        }
        return;
    }

//    if(SourceInfo::firstGreatOrEqualsSecond(serverVersion,readyVersion)){
//        serverVersion->installGame();
//        return;
//    }

    if(m_sourceLocal != nullptr){
        m_sourceLocal->installGame();
        return;
    }

//    if(SourceInfo::firstGreatSecond(m_sourceServer,actualVersion)){
//        m_sourceServer->installGame();
//        return;
//    }
    if(m_sourceServer->version() != actualVersion->version()){
        m_sourceLocal = m_sourceServer->clone();
        m_sourceLocal->installGame();
        return;
    }
}

void LauncherPlace::update()
{
    QProcess updateProcess;

    std::stringstream sstr;
    sstr << GetCurrentProcessId();
    std::string str = sstr.str();

    if(QFile::exists(QDir::toNativeSeparators(m_sourceLocal->savePath() + "/Updater.exe")))
        QFile::remove(QDir::toNativeSeparators(m_sourceLocal->savePath() + "/Updater.exe"));

    QFile::copy(QDir::toNativeSeparators(m_sourceLocal->savePath() + "/" + m_sourceLocal->torrentInfo().rootFolder() + "/Updater.exe")
                ,QDir::toNativeSeparators(m_sourceLocal->savePath() + "/Updater.exe"));

    qDebug() <<  "Start launcher update";
    qDebug() <<  "Run " + QDir::toNativeSeparators(m_sourceLocal->savePath() + "/Updater.exe");
    qDebug() <<  "Arg 1 " + QDir::toNativeSeparators(m_sourceLocal->savePath() + "/" + m_sourceLocal->torrentInfo().rootFolder());
    qDebug() <<  "Arg 2 " + QDir::toNativeSeparators(QDir::currentPath());
    qDebug() <<  "Arg 3 " + QDir::toNativeSeparators(QDir::currentPath() + "/Launcher.exe");

    QString run = QDir::toNativeSeparators(m_sourceLocal->savePath() + "/Updater.exe");
    QString arg1 = QDir::toNativeSeparators(m_sourceLocal->savePath() + "/" + m_sourceLocal->torrentInfo().rootFolder());
    QString arg2 = QDir::toNativeSeparators(QDir::currentPath());
    QString arg3 = QDir::toNativeSeparators(QDir::currentPath() + "/Launcher.exe");

    //std::string cons = '"' + run.toStdString() + '"' + " " + '"' + arg1.toStdString() + '"' + " " + '"' + arg2.toStdString() + '"' + " " + '"' + arg3.toStdString() + '"';

    //qDebug() << QString::fromUtf8(cons.c_str());
    //system(cons.c_str());

    updateProcess.startDetached(QDir::toNativeSeparators(m_sourceLocal->savePath() + "/Updater.exe")
                           ,QStringList()
                           <<QDir::toNativeSeparators(m_sourceLocal->savePath() + "/" + m_sourceLocal->torrentInfo().rootFolder())
                           <<QDir::toNativeSeparators(QDir::currentPath())
                           <<QDir::toNativeSeparators(QDir::currentPath() + "/Launcher.exe") );

    Core::Launcher::instance()->quit();
}
