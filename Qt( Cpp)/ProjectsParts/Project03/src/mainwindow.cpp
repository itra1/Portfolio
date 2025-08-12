#include "mainwindow.h"
#include <QTextCodec>
#include <QTimer>
#include <QWebEngineCookieStore>
#include <QWebEngineNewWindowRequest>
#include <QWebEnginePage>
#include <QWebEngineProfile>
#include <QWebEngineSettings>
#include <QWebEngineView>
#include "application.h"
#include "ui_mainwindow.h"
#include "windows.h"

#define AUTH_SCRIPT "script"

MainWindow::MainWindow(Core::Application *app, Core::BrowserState *state, QWidget *parent)
    : QMainWindow(parent), 
    _app{app},
    _ui{new Ui::MainWindow},
    _browserState{state}
{
    _ui->setupUi(this);

    auto browserSettings = _ui->browser->settings();

    auto browserPage = _ui->browser->page();
    connect(browserPage, &QWebEnginePage::urlChanged, this, &MainWindow::UrlChangedSlot);
    connect(browserPage, &QWebEnginePage::loadProgress, this, &MainWindow::LoadProgress);
    connect(browserPage, &QWebEnginePage::loadStarted, this, &MainWindow::LoadStart);
    connect(browserPage, &QWebEnginePage::loadFinished, this, &MainWindow::LoadFinish);
    connect(browserPage, &QWebEnginePage::scrollPositionChanged, this, &MainWindow::ScrollPointChange);
    connect(browserPage, &QWebEnginePage::contentsSizeChanged, this, &MainWindow::ContentsSizeChanged);
    connect(browserPage, &QWebEnginePage::titleChanged, this, &MainWindow::TitleChanged);
    connect(browserPage,
            &QWebEnginePage::newWindowRequested,
            this,
            [=](QWebEngineNewWindowRequest &req)
            { _ui->browser->load(req.requestedUrl()); });

    setWindowFlags(Qt::FramelessWindowHint | Qt::WindowMinimizeButtonHint | Qt::Window);
}

MainWindow::~MainWindow()
{
    delete _ui;
}

void MainWindow::SetWindowTitle(QString title)
{
    setWindowTitle(title);
}

void MainWindow::SetState(CnpApi::State *state)
{
    _state = state;
}

void MainWindow::SetMaterial(CnpApi::Browser *browser)
{
    _browser = browser;
    QString targetUrl = browser->Url();

    if (_browser->Auth() != nullptr && _browser->Auth()->AuthType() == AUTH_SCRIPT)
    {
        _waitAuth = true;
        targetUrl = _browser->Auth()->Url();
    }

    LoadUrl(targetUrl);
}

CnpApi::Browser *MainWindow::Browser()
{
    return _browser;
}

Ui::MainWindow *MainWindow::Ui()
{
    return _ui;
}

void MainWindow::LoadUrl(const QUrl &url)
{
    _ui->browser->load(url);
}

void MainWindow::UrlChangedSlot(const QUrl &url)
{
    _browserState->setUrl(url.toString());
}
void MainWindow::LoadStart() {
    _browserState->setLoadingStatus(Core::BrowserState::LoadStart);
}

void MainWindow::LoadProgress(int progress) {
    _browserState->setLoadingStatus(Core::BrowserState::LoadProgress);
}

void MainWindow::LoadFinish(bool ok)
{
    if (ok)
    {
        if (_waitAuth)
        {
            Authorization();
            _waitAuth = false;
            return;
        }

        if (_state != nullptr) {
            if (_state->Url() != "" && !_state->UrlUse()) {
                _state->SetUrlUse(true);
                if (_state->Url() != GetUrl().toString()) {
                    qDebug() << "change url " << GetUrl().toString() << " to " << _state->Url();
                    LoadUrl(_state->Url());
                }
                return;
            }
            if (_state->Scale() != 0) {
                _ui->browser->page()->setZoomFactor(_state->Scale());
            }
            // if (_state->Scale() > 0) {
            //     for (int i = 0; i < _state->Scale(); i++)
            //         _browserActions->ZoomPlus();
            // } else {
            //     for (int i = 0; i >= _state->Scale(); i--)
            //         _browserActions->ZoomMinus();
            // }

            if (_state->ScrollY() != 0 || _state->ScrollX() != 0) {
                // QTimer::singleShot(0,this,SLOT(exitApp()));
                // QTimer::singleShot(1000, this, [&]() {
                //     auto scrollValue = _ui->browser->page()->scrollPosition();
                //     scrollValue.setY(scrollValue.y() + (_state->ScrollY()));
                //     scrollValue.setX(scrollValue.x() + (_state->ScrollX()));
                //     _browserActions->SetScroll(scrollValue);
                // });
            }
            _state = nullptr;
        }
    }
    _browserState->setLoadingStatus(Core::BrowserState::LoadComplete);
}

void MainWindow::ScrollPointChange(QPointF point)
{
    // qDebug() << " point " << point;
    _browserState->setScrollPoint(point);
}

void MainWindow::ContentsSizeChanged(QSizeF size)
{
    _browserState->setContentSize(size);
}

void MainWindow::TitleChanged(QString title)
{
    _browserState->setTitle(title);
}

void MainWindow::SetBrowserAction(BrowserAction * handler)
{
    _browserActions = handler;
}

QUrl MainWindow::GetUrl()
{
    return _ui->browser->page()->url();
}

bool MainWindow::IsIms()
{
    return _ui->browser->page()->url().url().contains("ims.ac.gov.ru");
}

bool MainWindow::IsBi()
{
    return _ui->browser->page()->url().url().contains("bi.ac.gov.ru");
}

void MainWindow::Authorization()
{
    _browserActions->ExecJs(_browser->Auth()->AuthScript().toStdString());
}

// bool MainWindow::nativeEvent(const QByteArray &eventType, void *message, long *result)
// {
//     Q_UNUSED(eventType)
//     Q_UNUSED(result)
//     // Преобразуем указатель message в MSG WinAPI
//     MSG *msg = reinterpret_cast<MSG *>(message);

//     qDebug() << "nativeEvent!";
//     qDebug() << "nativeEvent: " << msg->message;

//     // Если сообщение является HotKey, то ...
//     if (msg->message == WM_HOTKEY) {
//         // ... проверяем идентификатор HotKey
//         if (msg->wParam == 100) {
//             // Сообщаем об этом в консоль
//             qDebug() << "HotKey worked";
//             return true;
//         }
//     }
//     return false;
// }
