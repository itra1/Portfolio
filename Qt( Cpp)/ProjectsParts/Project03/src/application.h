#include "logger.h"
#include "mainwindow.h"
#include "socketclient.h"
#include <CnpPipe>
#include <network.h>
#include <QObject>
#include "pipehandler.h"
#include "browseraction.h"
#include "windows.h"
#include "winuser.h"
#include "browserstate.h"

#ifndef APPLICATION_H
#define APPLICATION_H

namespace Core {

class Application : public QObject
{
    Q_OBJECT
public:
    explicit Application();

    void Connect();
    void ShowWindow();

public slots:
    void OnMessageSlot(QString event, QString value);

private:
    Netlib::Network _network;
    BrowserState *_browserState;

    QString _id;
    int _pipeTimeout{0};
    int _applicationTimeout{0};

    BrowserAction *_browserActions;
    Logger *_logger;
    MainWindow *_window;
    CnpApi::Browser *_browser;
    CnpApi::State *_state;
    Pipe::Client *_pipeClient;
    PipeHandler *_pipeHandler;
    BrowserAction *_actions;
    void SetKeyboardLayout(DWORD key);
};
} // namespace Core
#endif // APPLICATION_H
