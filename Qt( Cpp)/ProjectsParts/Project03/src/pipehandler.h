#ifndef PIPEHANDLER_H
#define PIPEHANDLER_H

#include <QJsonObject>
#include <QObject>
#include <QTimer>
#include "pipeactions.h"
#include <CnpPipe>

class PipeHandler : public QObject
{
    Q_OBJECT
public:
    PipeHandler(Pipe::Client *pipeClient);

    //! Установка таймаута при отсутствии входящих пакетов
    void pipeTimeOut(int timeout);
    //! Установ таймаута при отсутствии каких либо пакетов
    void initiateTimeOut(int timeout);

    void Send(QString action);
    void Send(QString action, QString value);
    void Send(QString action, QJsonObject value);

public slots:
    void OnMessageSlot(QString event);

signals:
    void OnReceive(QString event, QString value);

private:
    Pipe::Client *_pipeClient;
    QTimer *_timer;

    int _pipeTimeout;
    int _initializeTimeout;
};

#endif // PIPEHANDLER_H
