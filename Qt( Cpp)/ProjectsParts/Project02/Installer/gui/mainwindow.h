#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>

namespace Ui {
class MainWindow;
}

class InstallDialog;

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    explicit MainWindow(QWidget *parent = nullptr);
    ~MainWindow();

private slots:
    void on_installButton_clicked();

    void on_changeDirectoryButton_clicked();

    void on_pushButton_clicked();

    void on_directoryField_textChanged(const QString &arg1);

private:
    Ui::MainWindow *m_ui;

    void onLoadTorrent();
    void setInstallPath(QString path);
    QString getInstallPath();

    void setInstallTextButton();
    void setLabelSpace();
    qint64 getDiskSpace();
};

#endif // MAINWINDOW_H
