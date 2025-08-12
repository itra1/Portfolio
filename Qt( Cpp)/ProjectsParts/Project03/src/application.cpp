#include "application.h"
#include <QJsonDocument>
#include <QJsonObject>
#include "pipeactions.h"
#include <CnpApi>

namespace Core
{
Application::Application()
    : QObject(nullptr)
{
    const QStringList args = QCoreApplication::arguments();

    for (const QString &arg : args.mid(1)) {
        qDebug() << "Argument: " << arg;
        if (arg.contains("-id")) {
            _id = arg.split("=")[1];
            _logger = new Logger(_id);
        }
        if (arg.contains("-initializetimeout")) {
            _applicationTimeout = arg.split("=")[1].toInt();
        }
        if (arg.contains("-pipetimeout")) {
            _pipeTimeout = arg.split("=")[1].toInt();
        }
    }

    _pipeClient = new Pipe::Client();
    _pipeClient->Connect(_id);

    _browserState = new BrowserState();

    _pipeHandler = new PipeHandler(_pipeClient);
    _pipeHandler->initiateTimeOut(_applicationTimeout);
    _pipeHandler->pipeTimeOut(_pipeTimeout);
    connect(_pipeHandler, &PipeHandler::OnReceive, this, &Core::Application::OnMessageSlot);
}

    void Application::Connect()
    {
        _pipeClient = new Pipe::Client();
        _pipeClient->Connect(_id);
    }

    void Application::ShowWindow()
    {
        _window = new MainWindow(this,_browserState);
        _window->show();
        _window->SetWindowTitle(_id);
    }

    void Application::OnMessageSlot(QString event, QString value)
    {
        if (event == PIPE_MATERIAL)
        {
            _browser = new CnpApi::Browser(QJsonDocument::fromJson(value.toUtf8()).object());
            _window->SetMaterial(_browser);

            _browserActions = new BrowserAction(_window->Browser(),_browserState, _window->Ui());
            _browserActions->SetPipeHandler(_pipeHandler);

            _window->SetBrowserAction(_browserActions);

            connect(_pipeHandler, &PipeHandler::OnReceive, _browserActions, &BrowserAction::ActionSlot);

            _pipeHandler->Send(PIPE_OK);
        }
        if (event == PIPE_CLOSE)
        {
            _pipeHandler->Send(PIPE_OK);
            QApplication::quit();
        }
        if (event == PIPE_KeyboardLayout) {
            try{
                auto convert = std::stoul(value.toStdString());
                SetKeyboardLayout(convert);
            }catch(std::exception ex){
                qDebug() << "exception " << ex.what();
            }
            _pipeHandler->Send(PIPE_OK);
        }

        if (event == PIPE_STATE)
        {
            if (value == "null" || value == nullptr || value == "")
                _state = nullptr;
            else
                _state = new CnpApi::State(QJsonDocument::fromJson(value.toUtf8()).object());
            _window->SetState(_state);
            _pipeHandler->Send(PIPE_OK);
        }
    }

    //! Установка клавиатуры
    //! https://sheroz.com/pages/blog/programmatically-switching-keyboard-layouts.html
    void Application::SetKeyboardLayout(DWORD key){
        int nBuff;
        nBuff=::GetKeyboardLayoutList(0, NULL);
        HKL * phkl = new HKL[nBuff];
        ::GetKeyboardLayoutList(nBuff,phkl);

        for(int nKeyboard=0; nKeyboard<nBuff; nKeyboard++){

#pragma warning( push )
#pragma warning(disable:4311)
            DWORD prop = (DWORD)phkl[nKeyboard];
#pragma warning(pop)

            if(key == prop){
                ::ActivateKeyboardLayout(phkl[nKeyboard], KLF_SETFORPROCESS);
            }

        }
    }

} // namespace Core
