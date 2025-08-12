#include "application.h"

#include <QFontDatabase>
#include <QTimer>
#include <QProcess>
#include <QList>
#include <QFile>
#include <QFileInfo>
#include <QString>
#include <QSettings>
#include <QTranslator>
#include <QStringList>

#include "config.h"
#include "gui/mainwindow.h"
#include "gui/installdialog.h"
#include "gui/completedialog.h"
#include "core/copytheards.h"
#include "base/bittorrent/session.h"
#include "base/bittorrent/torrenthandle.h"
#include "base/net/downloadhandler.h"
#include "shlobj_core.h"

#include <windows.h>
#include <stdio.h>
#include <accctrl.h>
#include <aclapi.h>

Application *Application::m_instance = nullptr;

Application::Application(QObject *parent)
    : QObject(parent)
    , m_load(false)
    , m_install(false)
    , m_runProcess(new QProcess())
    , m_isLoadIni(false)
    , m_isLoadTorrent(false)
{
    qDebug() << "startPric";

//    int fontId = QFontDatabase::addApplicationFont(":///font/Roboto-Regular.ttf");
//    QString family = QFontDatabase::applicationFontFamilies(fontId).at(0); //имя шрифта
//    QFont f(family);  // QFont c вашим шрифтом

    m_copyTheard = new QThread(this);
    m_copyTheardObject = new CopyTheards();

    connect(m_copyTheard,&QThread::started,m_copyTheardObject,&CopyTheards::run);
    connect(m_copyTheard,&QThread::finished,this,&Application::handleTheardFinished);
    connect(m_copyTheardObject,&CopyTheards::onFinished,m_copyTheard,&QThread::terminate);
    connect(m_copyTheardObject,&CopyTheards::onProgress,this,&Application::setProgress);
    connect(m_copyTheardObject,&CopyTheards::onStartCopy,this,&Application::handleStartCopy);

    qDebug() << QDir::current();

}

void Application::initInstance()
{
    if(!m_instance)
        m_instance = new Application();
}

void Application::freeInstance()
{
    if (m_instance) {
        delete m_instance;
        m_instance = nullptr;
    }
}

Application *Application::instance()
{
    return m_instance;
}

int Application::initiate(int argc, char *argv[])
{
    QApplication a(argc, argv);

//    for(auto fi : QDir::drives())
//        qDebug() << fi.absolutePath();

    QTranslator qtTranslator;

    QString fileLocal = "";

    if(QLocale::system().language() == QLocale::Russian){
        fileLocal = "installer_ru_RU";
    }else{
        fileLocal = "installer_en_EN";
    }

    qtTranslator.load(fileLocal,"./localization/");
    a.installTranslator(&qtTranslator);

    Profile::initialize(Config::TMP_PATH_ROOT, nullptr,true);
    BitTorrent::Session::initInstance();

    connect(BitTorrent::Session::instance(),&BitTorrent::Session::torrentAdded,
            this,&Application::handleTorrentAdded);
    connect(BitTorrent::Session::instance(),&BitTorrent::Session::torrentFinished,
            this,&Application::handleTorrentFinished);
    connect(BitTorrent::Session::instance(),&BitTorrent::Session::torrentFinishedChecking,
            this,&Application::handleTorrentFinishedChecking);

    m_mainDlg = new MainWindow;
    m_mainDlg->show();

    return a.exec();
}

void Application::downloadIni()
{

    QString urlSource = QString("%1/%2").arg(Config::SERVER_LAUNCHER_RELEASE_TORRENT).arg(Config::SERVER_FILE_INFO);

    Net::DownloadHandler *handler = Net::DownloadManager::instance()->download(Net::DownloadRequest(urlSource));
    connect(handler, static_cast<void (Net::DownloadHandler::*)(const QString &, const QByteArray &)>(&Net::DownloadHandler::downloadFinished)
            , this, &Application::handleDownloadIniComplete);
    connect(handler, &Net::DownloadHandler::downloadFailed, this, &Application::handleDownloadIniFailed);

}

void Application::downloadTorrent()
{

    QString urlSource = QString("%1/%2").arg(Config::SERVER_LAUNCHER_RELEASE_TORRENT).arg(Config::SERVER_FILE_TORRENT);

    Net::DownloadHandler *handler = Net::DownloadManager::instance()->download(Net::DownloadRequest(urlSource));
    connect(handler, static_cast<void (Net::DownloadHandler::*)(const QString &, const QByteArray &)>(&Net::DownloadHandler::downloadFinished)
            , this, &Application::handleDownloadTorrentComplete);
    connect(handler, &Net::DownloadHandler::downloadFailed, this, &Application::handleDownloadTorrentFailed);

}

void Application::handleDownloadTorrentComplete(const QString &url, const QByteArray &data)
{
    m_byteTorrent = data;
    m_torrent = BitTorrent::TorrentInfo::load(data);
    qDebug() << m_torrent.rootFolder();
    m_load = true;

    emit onLoadTorrent();

    if(m_install){
        playTorrent();
    }
    m_isLoadTorrent = true;
    checkMoveIni();
}

void Application::handleDownloadTorrentFailed(const QString &url, const QString &reason)
{
    qDebug() << "torrent failed";
    QTimer::singleShot(1000,this,&Application::downloadTorrent);

}

BitTorrent::TorrentInfo Application::torrent() const
{
    return m_torrent;
}

void Application::install(QString path)
{
    m_installDlg = new InstallDialog();
    m_installDlg->show();
    m_mainDlg->close();

    m_installPath = QDir::fromNativeSeparators(path);

//    if(QDir(Config::TMP_PATH_ROOT + "/launcher").exists())
//        QDir(Config::TMP_PATH_ROOT + "/launcher").removeRecursively();

    if(!QDir(m_installPath).exists())
        QDir(m_installPath).mkpath(m_installPath);

    if(m_installPath[m_installPath.length()-1] == "/" || m_installPath[m_installPath.length()-1] == "\\")
            m_installPath = m_installPath.remove(m_installPath.length()-1,1);

    setPermission();

    if(!m_load){
        m_install = true;
        return;
    }

    playTorrent();
}

void Application::complete()
{
    createIcons();
    m_completeDlg = new CompleteDialog();
    m_completeDlg->show();
    m_installDlg->close();
}

void Application::quit()
{
    BitTorrent::Session::freeInstance();
    QApplication::exit();
}

void Application::handleTorrentAdded(BitTorrent::TorrentHandle * const torrent)
{
    qDebug() << "handleTorrentAdded";
}

void Application::handleTorrentFinished(BitTorrent::TorrentHandle * const torrent)
{
    if(m_copyTheard->isRunning())
        return;

    qDebug() << "handleTorrentFinished";

    QString sourceDir = Config::TMP_PATH_ROOT + "/launcher/" + m_torrent.rootFolder();
    QString targetDir = m_installPath;

    m_copyTheardObject->setFromPath(sourceDir);
    m_copyTheardObject->setToPath(targetDir);

    m_copyTheard->start();
}

void Application::handleTorrentFinishedChecking(BitTorrent::TorrentHandle * const torrent)
{
    qDebug() << "handleTorrentFinishedChecking " << torrent->progress() << " " << torrent->isCompleted();
    if(torrent->progress() >= 0.94)
        handleTorrentFinished(torrent);
}

void Application::setProgress(double progress)
{
    emit onProgressCopy(progress);
}

void Application::handleTheardFinished()
{
    complete();
}

void Application::handleStartCopy()
{

}

QString Application::runFile() const
{
    return m_runFile;
}

void Application::setRunFile(const QString &runFile)
{
    m_runFile = runFile;
}

void Application::createIcons()
{
    TCHAR system_folder[MAX_PATH];
    SHGetSpecialFolderPath(0,system_folder, CSIDL_DESKTOPDIRECTORY,true);
    std::wstring wStr = system_folder;
    std::string installPath = std::string(wStr.begin(), wStr.end());

    QFile(m_installPath + m_runFile).link(QString::fromStdString(installPath) + "/Population Zero.lnk");

    system_folder[MAX_PATH];
    SHGetSpecialFolderPath(0,system_folder, CSIDL_STARTMENU,true);
    wStr = system_folder;
    installPath = std::string(wStr.begin(), wStr.end());

    QString newPath = QDir::toNativeSeparators(QString::fromStdString(installPath) + "/Population Zero");

    if(!QDir(newPath).exists())
        QDir(newPath).mkpath(newPath);

    QFile(m_installPath + m_runFile).link(newPath + "/Population Zero.lnk");
    qDebug() << QString::fromStdString(installPath);
}

QString Application::installPath() const
{
    return m_installPath;
}

void Application::setInstallPath(const QString &installPath)
{
    m_installPath = installPath;
}

void Application::handleDownloadIniComplete(const QString &url, const QByteArray &data)
{
    QString filePath = Config::TMP_PATH_ROOT + "/launcher";

    if(!QDir().exists(filePath))
        QDir().mkpath(filePath);

    QString writeFile = Utils::Fs::toNativePath(filePath + "/source.ini");

    QFile saveFile(writeFile);

    if(saveFile.exists())
       saveFile.remove();

    saveFile.open(QIODevice::WriteOnly);
    saveFile.write(data);
    saveFile.close();

    QSettings *set = new QSettings(writeFile,QSettings::IniFormat);
    m_runFile = set->value("runFile").toString();

    m_isLoadIni = true;
    checkMoveIni();
}

void Application::handleDownloadIniFailed(const QString &url, const QString &reason)
{
    QTimer::singleShot(1000,this,&Application::downloadIni);
}

bool Application::load() const
{
    return m_load;
}

void Application::play()
{

    m_runProcess->startDetached("\""+ QDir::toNativeSeparators(m_installPath + m_runFile)+"\" setRunPath \""+ QDir::toNativeSeparators(m_installPath) +"\"");
    qDebug() << QDir::toNativeSeparators(m_installPath + m_runFile);

    if( !m_runProcess->waitForStarted() || !m_runProcess->waitForFinished() ) {
            return;
        }

    qDebug() << m_runProcess->readAllStandardOutput();

    //quit();
}

void Application::playTorrent()
{
    if(!QDir(m_installPath).exists())
        QDir(m_installPath).mkdir(m_installPath);

    BitTorrent::AddTorrentParams params;
    params.name = "louncher";
    params.version = "release";
    params.savePath = Config::TMP_PATH_ROOT + "/launcher";
    params.sequential = true;
    params.skipChecking = false;

    BitTorrent::Session::instance()->addTorrent(params,m_byteTorrent);
}

void Application::checkMoveIni()
{
    if(!m_isLoadIni || !m_isLoadTorrent)
        return;

    QString sourceDir = Config::TMP_PATH_ROOT + "/launcher";
    QString targetDir = Config::TMP_PATH_ROOT + "/launcher/" + m_torrent.rootFolder();
    QString fileIni = "/source.ini";
    QString fileRes = "/source.res";

    if(!QDir(targetDir).exists())
        QDir(targetDir).mkpath(targetDir);

    QString sourceFile = sourceDir + fileIni;
    QString targetFile = targetDir + fileIni;

    if(QFile::exists(targetFile))
        QFile::remove(targetFile);

    QString targetTorrent = targetDir + fileRes;

    if(QFile::exists(targetTorrent))
        QFile::remove(targetTorrent);

    QFile(sourceFile).rename(targetFile);
}

//namespace  {

//BOOL SetPrivilege(
//    HANDLE hToken,          // access token handle
//    LPCTSTR lpszPrivilege,  // name of privilege to enable/disable
//    BOOL bEnablePrivilege   // to enable or disable privilege
//    ) ;
//}

bool Application::SetPrivilege(
    HANDLE hToken,          // access token handle
    LPCTSTR lpszPrivilege,  // name of privilege to enable/disable
    BOOL bEnablePrivilege   // to enable or disable privilege
    )
{
    TOKEN_PRIVILEGES tp;
    LUID luid;

    if ( !LookupPrivilegeValue(
            NULL,            // lookup privilege on local system
            lpszPrivilege,   // privilege to lookup
            &luid ) )        // receives LUID of privilege
    {
        printf("LookupPrivilegeValue error: %u\n", GetLastError() );
        return FALSE;
    }

    tp.PrivilegeCount = 1;
    tp.Privileges[0].Luid = luid;
    if (bEnablePrivilege)
        tp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
    else
        tp.Privileges[0].Attributes = 0;

    // Enable the privilege or disable all privileges.

    if ( !AdjustTokenPrivileges(
           hToken,
           FALSE,
           &tp,
           sizeof(TOKEN_PRIVILEGES),
           (PTOKEN_PRIVILEGES) NULL,
           (PDWORD) NULL) )
    {
          printf("AdjustTokenPrivileges error: %u\n", GetLastError() );
          return FALSE;
    }

    if (GetLastError() == ERROR_NOT_ALL_ASSIGNED)

    {
          printf("The token does not have the specified privilege. \n");
          return FALSE;
    }

    return TRUE;
}


void Application::setPermission()
{
//    TCHAR system_folder[MAX_PATH];
//    SHGetSpecialFolderPath(0,system_folder, CSIDL_PROGRAM_FILES,true);
//    std::wstring wStr = system_folder;
//    std::string installPath = std::string(wStr.begin(), wStr.end());

    //char path[] = "C:\Program Files\Population Zero";
//    A2W(path)
//    std::wstring wPath = std::wstring(path.begin(),path.end());
//    LPWSTR pp = wPath.c_str();
//    LPWSTR lp = A2W_EX(path.c_str(), path.length());

//    std::string str = "String to LPWSTR";
//     BSTR b = _com_util::ConvertStringToBSTR(str.c_str());
//     LPWSTR lp = b;

     LPWSTR strVariable2 = (wchar_t*) m_installPath.utf16();

    BOOL bRetval = FALSE;

    HANDLE hToken = NULL;
    PSID pSIDAdmin = NULL;
    PSID pSIDEveryone = NULL;
    PACL pACL = NULL;
    SID_IDENTIFIER_AUTHORITY SIDAuthWorld =
            SECURITY_WORLD_SID_AUTHORITY;
    SID_IDENTIFIER_AUTHORITY SIDAuthNT = SECURITY_NT_AUTHORITY;
    const int NUM_ACES  = 2;
    EXPLICIT_ACCESS ea[NUM_ACES];
    DWORD dwRes;

    if (!AllocateAndInitializeSid(&SIDAuthWorld, 1,
                         SECURITY_WORLD_RID,
                         0,
                         0, 0, 0, 0, 0, 0,
                         &pSIDEveryone))
        {
            printf("AllocateAndInitializeSid (Everyone) error %u\n",
                    GetLastError());
            goto Cleanup;
        }

    // Create a SID for the BUILTIN\Administrators group.
        if (!AllocateAndInitializeSid(&SIDAuthNT, 2,
                         SECURITY_BUILTIN_DOMAIN_RID,
                         DOMAIN_ALIAS_RID_ADMINS,
                         0, 0, 0, 0, 0, 0,
                         &pSIDAdmin))
        {
            printf("AllocateAndInitializeSid (Admin) error %u\n",
                    GetLastError());
            goto Cleanup;
        }

        ZeroMemory(&ea, NUM_ACES * sizeof(EXPLICIT_ACCESS));

        // Set read access for Everyone.
        ea[0].grfAccessPermissions = GENERIC_ALL;
        ea[0].grfAccessMode = SET_ACCESS;
        ea[0].grfInheritance = SUB_CONTAINERS_AND_OBJECTS_INHERIT;
        ea[0].Trustee.TrusteeForm = TRUSTEE_IS_SID;
        //ea[0].Trustee.TrusteeType = TRUSTEE_IS_WELL_KNOWN_GROUP;
        ea[0].Trustee.TrusteeType = TRUSTEE_IS_GROUP;
        ea[0].Trustee.ptstrName = (LPTSTR) pSIDEveryone;

        // Set full control for Administrators.
        ea[1].grfAccessPermissions = GENERIC_ALL;
        ea[1].grfAccessMode = SET_ACCESS;
        ea[1].grfInheritance = NO_INHERITANCE;
        ea[1].Trustee.TrusteeForm = TRUSTEE_IS_SID;
        ea[1].Trustee.TrusteeType = TRUSTEE_IS_GROUP;
        ea[1].Trustee.ptstrName = (LPTSTR) pSIDAdmin;

        if (ERROR_SUCCESS != SetEntriesInAcl(NUM_ACES,
                                             ea,
                                             NULL,
                                             &pACL))
        {
            printf("Failed SetEntriesInAcl\n");
            goto Cleanup;
        }

        // Try to modify the object's DACL.
        dwRes = SetNamedSecurityInfo(
            strVariable2,                 // name of the object
            SE_FILE_OBJECT,              // type of object
            DACL_SECURITY_INFORMATION,   // change only the object's DACL
            NULL, NULL,                  // do not change owner or group
            pACL,                        // DACL specified
            NULL);                       // do not change SACL

        if (ERROR_SUCCESS == dwRes)
        {
            printf("Successfully changed DACL\n");
            bRetval = TRUE;
            // No more processing needed.
            goto Cleanup;
        }
        if (dwRes != ERROR_ACCESS_DENIED)
        {
            printf("First SetNamedSecurityInfo call failed: %u\n",
                    dwRes);
            goto Cleanup;
        }

        // If the preceding call failed because access was denied,
        // enable the SE_TAKE_OWNERSHIP_NAME privilege, create a SID for
        // the Administrators group, take ownership of the object, and
        // disable the privilege. Then try again to set the object's DACL.

        // Open a handle to the access token for the calling process.
        if (!OpenProcessToken(GetCurrentProcess(),
                              TOKEN_ADJUST_PRIVILEGES,
                              &hToken))
           {
              printf("OpenProcessToken failed: %u\n", GetLastError());
              goto Cleanup;
           }

        // Enable the SE_TAKE_OWNERSHIP_NAME privilege.
        if (!SetPrivilege(hToken, SE_TAKE_OWNERSHIP_NAME, TRUE))
        {
            printf("You must be logged on as Administrator.\n");
            goto Cleanup;
        }

        // Set the owner in the object's security descriptor.
        dwRes = SetNamedSecurityInfo(
            strVariable2,                 // name of the object
            SE_FILE_OBJECT,              // type of object
            OWNER_SECURITY_INFORMATION,  // change only the object's owner
            pSIDAdmin,                   // SID of Administrator group
            NULL,
            NULL,
            NULL);

        if (dwRes != ERROR_SUCCESS)
        {
            printf("Could not set owner. Error: %u\n", dwRes);
            goto Cleanup;
        }

        // Disable the SE_TAKE_OWNERSHIP_NAME privilege.
        if (!SetPrivilege(hToken, SE_TAKE_OWNERSHIP_NAME, FALSE))
        {
            printf("Failed SetPrivilege call unexpectedly.\n");
            goto Cleanup;
        }

        // Try again to modify the object's DACL,
        // now that we are the owner.
        dwRes = SetNamedSecurityInfo(
            strVariable2,                 // name of the object
            SE_FILE_OBJECT,              // type of object
            DACL_SECURITY_INFORMATION,   // change only the object's DACL
            NULL, NULL,                  // do not change owner or group
            pACL,                        // DACL specified
            NULL);                       // do not change SACL

        if (dwRes == ERROR_SUCCESS)
        {
            printf("Successfully changed DACL\n");
            bRetval = TRUE;
        }
        else
        {
            printf("Second SetNamedSecurityInfo call failed: %u\n",
                    dwRes);
        }

    Cleanup:

        if (pSIDAdmin)
            FreeSid(pSIDAdmin);

        if (pSIDEveryone)
            FreeSid(pSIDEveryone);

        if (pACL)
           LocalFree(pACL);

        if (hToken)
           CloseHandle(hToken);

}
