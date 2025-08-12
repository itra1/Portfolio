#include "completedialog.h"
#include "ui_completedialog.h"
#include "core/application.h"
#include <QProcess>
#include <QDir>
#include <QRegExp>
#include <QDebug>

CompleteDialog::CompleteDialog(QWidget *parent) :
    QWidget(parent, Qt::Window | Qt::FramelessWindowHint),
    m_ui(new Ui::CompleteDialog)
{
    m_ui->setupUi(this);
    setAttribute(Qt::WA_TranslucentBackground, true);
    this->setStyleSheet("#mainFrame{background-color: #2d2d2d; border: 1px solid #3b3b3b; border-radius: 8;}");
}

CompleteDialog::~CompleteDialog()
{
    delete m_ui;
}

void CompleteDialog::on_playButton_clicked()
{
    this->close();
    Application::instance()->play();
}
