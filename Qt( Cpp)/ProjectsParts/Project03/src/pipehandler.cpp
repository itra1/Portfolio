#include "pipehandler.h"
#include <QApplication>

PipeHandler::PipeHandler(Pipe::Client *pipeClient)
    : QObject(nullptr)
    , _pipeClient{pipeClient}
    , _timer{new QTimer()}
{
    connect(_pipeClient,
            &Pipe::Client::OnMessageSignal,
            this,
            &PipeHandler::OnMessageSlot,
            Qt::UniqueConnection);
    connect(
        _pipeClient,
        &Pipe::Client::OnConnectClose,
        this,
        [&]() {
            qDebug() << "Pipe close connection";
            QApplication::quit();
        },
        Qt::UniqueConnection);

    connect(_timer, &QTimer::timeout, this, [&]() {
        qDebug() << "Pipe time out !";
        QApplication::quit();
    });
}

void PipeHandler::pipeTimeOut(int timeout)
{
    _pipeTimeout = timeout;
}

void PipeHandler::initiateTimeOut(int timeout)
{
    _initializeTimeout = timeout;

    _timer->stop();
    if (_initializeTimeout > 0) {
        _timer->start(_initializeTimeout);
    }
}

void PipeHandler::Send(QString action)
{
    QJsonArray arr;
    arr.append(QJsonValue(action));
    _pipeClient->Write(QJsonDocument(arr).toJson(QJsonDocument::Compact));
}

void PipeHandler::Send(QString action, QString value)
{
    QJsonArray arr;
    arr.append(QJsonValue(action));
    arr.append(QJsonValue(value));
    _pipeClient->Write(QJsonDocument(arr).toJson(QJsonDocument::Compact));
}

void PipeHandler::Send(QString action, QJsonObject value)
{
    QJsonArray arr;
    arr.append(QJsonValue(action));
    arr.append(value);
    _pipeClient->Write(QJsonDocument(arr).toJson(QJsonDocument::Compact));
}

void PipeHandler::OnMessageSlot(QString message)
{
    _timer->stop();
    if (_pipeTimeout > 0) {
        _timer->start(_pipeTimeout);
    }

    try {
        QJsonArray arr = QJsonDocument::fromJson(message.toUtf8()).array();
        QString action = arr[0].toString();

        QString value = nullptr;

        if (arr.count() > 1) {
            if (arr[1].isString())
                value = arr[1].toString();
            else
                value = QJsonDocument(arr[1].toObject()).toJson(QJsonDocument::Compact);
        }

        emit OnReceive(action, value);
    } catch (std::exception ex) {
        qDebug() << ex.what();
        Send(PIPE_ERROR, QString::fromUtf8(ex.what()));
    }
}
