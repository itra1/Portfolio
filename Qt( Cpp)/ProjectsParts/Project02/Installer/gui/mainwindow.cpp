#include "mainwindow.h"
#include <QDebug>
#include "ui_mainwindow.h"
#include "core/application.h"
#include <QFileDialog>
#include "base/utils/misc.h"
#include "base/utils/fs.h"
#include "shlobj_core.h"

MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent, Qt::Window | Qt::FramelessWindowHint),
    m_ui(new Ui::MainWindow)
{
    m_ui->setupUi(this);
    setAttribute(Qt::WA_TranslucentBackground, true);
    this->setStyleSheet("#mainFrame{background-color: #2d2d2d; border: 1px solid #3b3b3b; border-radius: 8;}");

    // Set install dictionary
    TCHAR system_folder[MAX_PATH];
    SHGetSpecialFolderPath(0,system_folder, CSIDL_PROGRAM_FILES,true);
    std::wstring wStr = system_folder;
    std::string installPath = std::string(wStr.begin(), wStr.end());
    setInstallPath(QDir::toNativeSeparators(QString::fromStdString(installPath) + "/Population Zero"));

    setInstallTextButton();

    Application::instance()->downloadIni();
    Application::instance()->downloadTorrent();
    connect(Application::instance(),&Application::onLoadTorrent,this,&MainWindow::onLoadTorrent);

}

MainWindow::~MainWindow()
{
    delete m_ui;
}

void MainWindow::on_installButton_clicked()
{
    if(!Application::instance()->load())
        return;

    if(getDiskSpace() < Application::instance()->torrent().totalSize())
        return;

    Application::instance()->install(QDir::toNativeSeparators(getInstallPath()));
}

void MainWindow::on_changeDirectoryButton_clicked()
{
    QString installPath = getInstallPath();
    QStringList strList = installPath.split(QDir::separator());

    if(strList[strList.length()-1] == "Population Zero" || strList[strList.length()-1] == "PZ")
        strList.removeLast();

    installPath = strList.join(QDir::separator());

    if(strList.length() <= 1)
        installPath = installPath + QDir::separator();

    QString dir = QFileDialog::getExistingDirectory(nullptr, tr("Open Directory"),
                                                    installPath,
                                                    QFileDialog::ShowDirsOnly
                                                    | QFileDialog::DontResolveSymlinks);
    if(dir!= "")
        setInstallPath(QDir::toNativeSeparators(dir));
    setLabelSpace();
}

void MainWindow::onLoadTorrent()
{
    setInstallTextButton();
    setLabelSpace();
}

void MainWindow::setInstallPath(QString path)
{
    if(path[path.length()-1] == "\\")
        path = path.remove(path.length()-1,1);

    QStringList strList = path.split(QDir::separator());

    if(strList[strList.length()-1] != "Population Zero" && strList[strList.length()-1] != "PZ")
        path = path + QDir::separator() + "Population Zero";

    m_ui->directoryField->setText(QDir::toNativeSeparators(path));
}

QString MainWindow::getInstallPath()
{
    return m_ui->directoryField->text();
}

void MainWindow::setInstallTextButton()
{
    QString size = "";
//    bool load = Application::instance()->load();
//    if(load){
//        size = " ("+ Utils::Misc::friendlyUnit(Application::instance()->torrent().totalSize()) + ")";
//    }

    m_ui->installButton->setText(tr("Install launcher%1").arg(size));
}

void MainWindow::setLabelSpace()
{
    QString lbl = tr("Loading...");

    qint64 diskSpace = getDiskSpace();

    if(diskSpace == -1){
        m_ui->infoLabel->setText(tr("No correct path"));
        return;
    }

    bool load = Application::instance()->load();
    if(load){
        lbl = QString(tr("Available %1 of required %2"))
                .arg(Utils::Misc::friendlyUnit(diskSpace))
                .arg(Utils::Misc::friendlyUnit(Application::instance()->torrent().totalSize()));
    }

    m_ui->infoLabel->setText(lbl);
}

qint64 MainWindow::getDiskSpace()
{
    if(m_ui->directoryField->text().length() < 1){
        return -1;
    }
    QString disc = m_ui->directoryField->text().left(1);

    return Utils::Fs::freeDiskSpaceOnPath(disc + ":\\");
}

void MainWindow::on_pushButton_clicked()
{
    this->close();
    Application::instance()->quit();
}

void MainWindow::on_directoryField_textChanged(const QString &arg1)
{
    setLabelSpace();
}
