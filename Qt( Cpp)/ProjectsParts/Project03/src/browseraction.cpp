#include "browseraction.h"
#include "jsactions.h"
#include "mainwindow.h"
#include "pipeactions.h"

#define MATERIAL_TYPE_DOCUMENT "DOCUMENT"
#define MATERIAL_TYPE_PRESENTATION "PRESENTATION"
#define MATERIAL_TYPE_BROWSER "BROWSER"
#define MATERIAL_TYPE_WORD "WORD"
#define MATERIAL_SUBTYPE_EXCEL "EXCEL"
#define MATERIAL_SUBTYPE_FIGMA "FIGMA"
#define MATERIAL_SUBTYPE_MIRO "MIRO"

#define scrollIncrement 10
#define zoomIncrement 0.01

BrowserAction::BrowserAction(CnpApi::Browser *material, Core::BrowserState *browserState, Ui::MainWindow *ui)
    : QObject(nullptr)
    , _material{material}
    , _ui{ui}
    , _browserState{browserState}
{}

void BrowserAction::ActionSlot(QString action, QString value)
{
    if (action == PIPE_PING) {

        if(_browserState->existsChange()){
            QJsonObject state = _browserState->getStateJson();
            _pipeHandler->Send(PIPE_PONG, state);
            _browserState->resetExistsChange();
        }else{
            _pipeHandler->Send(PIPE_PONG);
        }
    }
    if (action == PIPE_ZoomPlus) {
        ZoomPlus();
        _pipeHandler->Send(PIPE_OK);
    }
    if (action == PIPE_ZoomMinus) {
        ZoomMinus();
        _pipeHandler->Send(PIPE_OK);
    }
    if (action == PIPE_Backward) {
        Backward();
        _pipeHandler->Send(PIPE_OK);
    }
    if (action == PIPE_Forward) {
        Forward();
        _pipeHandler->Send(PIPE_OK);
    }
    if (action == PIPE_Reload) {
        Reload();
        _pipeHandler->Send(PIPE_OK);
    }
    if (action == PIPE_ScrollUp) {
        ScrollUp();
        _pipeHandler->Send(PIPE_OK);
    }
    if (action == PIPE_ScrollDown) {
        ScrollDown();
        _pipeHandler->Send(PIPE_OK);
    }
    if (action == PIPE_RunJs) {
        ExecJs(value.toStdString());
        _pipeHandler->Send(PIPE_OK);
    }
    if (action == PIPE_RunJsReturn) {
        ExecJs(value.toStdString(), [&](QString result) {
            _pipeHandler->Send(PIPE_OK, result);
        });
    }
    if (action == PIPE_StateGet) {
        _pipeHandler->Send(PIPE_OK, MakeBrowserState());
    }
    if (action == PIPE_Url) {
        _ui->browser->load(value);
        _pipeHandler->Send(PIPE_OK);
    }
}

QJsonObject BrowserAction::MakeBrowserState()
{
    // Scale, Url, ScrollX, ScrollY
    QJsonObject json;
    json["Scale"] = _ui->browser->page()->zoomFactor();
    json["Url"] = _ui->browser->page()->url().toString();
    json["ScrollX"] = _ui->browser->page()->scrollPosition().x();
    json["ScrollY"] = _ui->browser->page()->scrollPosition().y();

    return json;
}

void BrowserAction::SetPipeHandler(PipeHandler *NewPipeHandler)
{
    _pipeHandler = NewPipeHandler;
}

void BrowserAction::ExecJs(std::string script)
{
    _ui->browser->page()->runJavaScript("(function () {" + QString::fromStdString(script) + "})()");
}

void BrowserAction::ExecJs(std::string script,
                           const std::function<void(const QString)> &resultCallback)
{
    try {
        _ui->browser->page()->runJavaScript(QString::fromStdString(script), [=](QVariant result) {
            try {
                resultCallback(result.toString());
            } catch (std::exception ex) {
                qDebug() << "Exception js" << ex.what();
            }
        });
    } catch (std::exception ex) {
        qDebug() << "Exception " << ex.what();
    }
}

void BrowserAction::ScrollUp()
{
    auto scrollValue = _ui->browser->page()->scrollPosition();
    scrollValue.setY(scrollValue.y() - scrollIncrement);
    SetScroll(scrollValue);
}

void BrowserAction::ScrollDown()
{
    auto scrollValue = _ui->browser->page()->scrollPosition();
    scrollValue.setY(scrollValue.y() - scrollIncrement);
    SetScroll(scrollValue);
}
void BrowserAction::SetScroll(QPointF point)
{
    _ui->browser->page()->scrollPositionChanged(point);
}

void BrowserAction::ZoomPlus()
{
    ZoomChange(zoomIncrement);
}

void BrowserAction::ZoomMinus()
{
    ZoomChange(-zoomIncrement);
}

void BrowserAction::ZoomChange(qreal value)
{
    auto zoom = _ui->browser->page()->zoomFactor();
    _ui->browser->page()->setZoomFactor(zoom +value);
    QTimer::singleShot(300,[=](){
        _browserState->setZoom(_ui->browser->page()->zoomFactor());
    });
}

void BrowserAction::Backward()
{
    _ui->browser->back();
}

void BrowserAction::Forward()
{
    _ui->browser->forward();
}

void BrowserAction::Reload()
{
    _ui->browser->reload();
}
