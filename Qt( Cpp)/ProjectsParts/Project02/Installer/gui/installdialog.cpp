#include "installdialog.h"
#include "ui_installdialog.h"
#include "core/application.h"
#include "base/bittorrent/session.h"
#include <QDebug>
#include <QHash>
#include "base/bittorrent/infohash.h"
#include "base/bittorrent/torrenthandle.h"

InstallDialog::InstallDialog(QWidget *parent) :
    QWidget(parent, Qt::Window | Qt::FramelessWindowHint),
    m_ui(new Ui::InstallDialog),
    m_installComplete(true)
{
    m_ui->setupUi(this);
    setAttribute(Qt::WA_TranslucentBackground, true);
    this->setStyleSheet("#mainFrame{background-color: #2d2d2d; border: 1px solid #3b3b3b; border-radius: 8;}");

    connect(BitTorrent::Session::instance(),&BitTorrent::Session::torrentsUpdated,
            this,&InstallDialog::handleTorrentsUpdated);
    connect(BitTorrent::Session::instance(),&BitTorrent::Session::torrentFinished,
            this,&InstallDialog::handleTorrentFinished);
    connect(Application::instance(),&Application::onProgressCopy,
            this,&InstallDialog::progressCopy);

//    m_updateTimer = new QTimer();
//    m_updateTimer->setInterval(1000);
//    connect(m_updateTimer, &QTimer::timeout,this, &InstallDialog::handleTorrentsUpdated);
//    m_updateTimer->start();

    setPercent(0);
}

InstallDialog::~InstallDialog()
{
    delete m_ui;
}

void InstallDialog::handleTorrentsUpdated()
{
    if(!m_installComplete)
        return;
    if(BitTorrent::Session::instance()->torrents().keys().length()<= 0)
        return;
    auto torrent = BitTorrent::Session::instance()->torrents().values()[0];

    setPercent(torrent->progress()*0.9);
}

void InstallDialog::progressCopy(double val)
{
    setPercent((val*0.1) + 0.9);
}

void InstallDialog::on_cancelButton_clicked()
{
    this->close();
    Application::instance()->quit();
}

void InstallDialog::handleTorrentFinished(BitTorrent::TorrentHandle * const torrent)
{
    m_installComplete = false;
}

void InstallDialog::setPercent(double perc)
{
    if(perc > 1)
        perc = 1;
    m_ui->progressLine->setFixedWidth(371 * perc);
    m_ui->progressValue->setText(QString::number(round(perc * 10000)/100) + "%");
    QRect rectLine = m_ui->progressLine->geometry();
    QRect rectValue = m_ui->progressValue->geometry();
    m_ui->progressValue->setVisible(perc > 0.3);
    m_ui->progressValue->setGeometry(rectLine.x()+rectLine.width()-100,rectValue.y(),rectValue.width(),rectValue.height());
    QWidget::repaint();
}
