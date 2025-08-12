#ifndef BROWSERACTION_H
#define BROWSERACTION_H

#include "pipehandler.h"
#include "ui_mainwindow.h"
#include <CnpApi>
#include <QJsonObject>
#include "browserstate.h"

class BrowserAction : public QObject
{
public:
    BrowserAction(CnpApi::Browser *material, Core::BrowserState *browserState, Ui::MainWindow *ui);

    void ExecJs(std::string script);
    void ExecJs(std::string script, const std::function<void(const QString)> &resultCallback);

public:
    QJsonObject MakeBrowserState();
    
public:
    void ScrollUp();
    void ScrollDown();
    void SetScroll(QPointF point);
    void ZoomPlus();
    void ZoomMinus();
    void ZoomChange(qreal value);
    void Backward();
    void Forward();
    void Reload();

    void SetPipeHandler(PipeHandler *NewPipeHandler);

public slots:
    void ActionSlot(QString action, QString value);

private:
    CnpApi::Browser *_material;
    Ui::MainWindow *_ui;
    PipeHandler *_pipeHandler;
    Core::BrowserState *_browserState;
};

#endif // BROWSERACTION_H
