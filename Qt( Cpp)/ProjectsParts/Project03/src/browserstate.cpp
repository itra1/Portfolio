#include "browserstate.h"

namespace Core{

BrowserState::BrowserState() {
    _state = new State();
}

QJsonObject BrowserState::getStateJson()
{
    QJsonObject scroll;
    scroll["x"] = _state->ScrollPoint.x();
    scroll["y"] = _state->ScrollPoint.y();

    QJsonObject contentSize;
    contentSize["x"] = _state->ContentSize.width();
    contentSize["y"] = _state->ContentSize.height();

    QJsonObject statusJson;
    //statusJson["title"] = _state->Title.;
    statusJson["url"] = _state->Url;
    statusJson["loadStatus"] = _state->LoadingStatus;
    statusJson["zoom"] = _state->Zoom;
    statusJson["scroll"] = scroll;
    statusJson["contentSize"] = contentSize;

    QJsonObject result;
    result["state"] = statusJson;

    return result;
}

bool BrowserState::existsChange()
{
    return _existsChange;
}

void BrowserState::resetExistsChange()
{
    _existsChange = false;
}

void BrowserState::setTitle(QString title)
{
    _state->Title = title;
    _existsChange = true;
}

void BrowserState::setUrl(QString url)
{
    _state->Url = url;
    _existsChange = true;
}

void BrowserState::setLoadingStatus(QString status)
{
    _state->LoadingStatus = status;
    _existsChange = true;
}

void BrowserState::setScrollPoint(QPointF point)
{
    _state->ScrollPoint = point;
    _existsChange = true;
}

void BrowserState::setContentSize(QSizeF size)
{
    _state->ContentSize = size;
    _existsChange = true;
}

void BrowserState::setZoom(qreal zoom)
{
    _state->Zoom = zoom;
    _existsChange = true;
}
}
