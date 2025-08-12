#ifndef COMPLETEDIALOG_H
#define COMPLETEDIALOG_H

#include <QWidget>

class QProcess;

namespace Ui {
class CompleteDialog;
}

class CompleteDialog : public QWidget
{
    Q_OBJECT

public:
    explicit CompleteDialog(QWidget *parent = nullptr);
    ~CompleteDialog();

private slots:
    void on_playButton_clicked();

private:
    Ui::CompleteDialog *m_ui;
    QProcess *m_p;
};

#endif // COMPLETEDIALOG_H
