#ifndef INSTALLDIALOG_H
#define INSTALLDIALOG_H

#include <QWidget>
#include <QTimer>

namespace Ui {
class InstallDialog;
}

namespace BitTorrent{
    class TorrentHandle;
}

class InstallDialog : public QWidget
{
    Q_OBJECT

public:
    explicit InstallDialog(QWidget *parent = nullptr);
    ~InstallDialog();

public slots:
    void handleTorrentsUpdated();
    void progressCopy(double val);

private slots:
    void on_cancelButton_clicked();
    void handleTorrentFinished(BitTorrent::TorrentHandle *const torrent);

private:
    Ui::InstallDialog *m_ui;
    void setPercent(double perc);
    QTimer *m_updateTimer;
    bool m_installComplete;
};

#endif // INSTALLDIALOG_H
