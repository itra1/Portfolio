#ifndef BROWSERSTATE_H
#define BROWSERSTATE_H

#include <QObject>
#include <QPointF>
#include <QSizeF>
#include <QJsonObject>

namespace Core {

class BrowserState
{
public:
    struct State{
        QString Title;
        QString Url;
        QString LoadingStatus;
        QPointF ScrollPoint;
        QSizeF ContentSize;
        qreal Zoom;
    };

    inline static const QString LoadStart{"LoadStart"};
    inline static const QString LoadProgress{"LoadProgress"};
    inline static const QString LoadComplete{"LoadComplete"};

public:
    explicit BrowserState();

public:

    QJsonObject getStateJson();

    bool existsChange();
    void resetExistsChange();

    void setTitle(QString title);
    void setUrl(QString url);
    void setLoadingStatus(QString status);
    void setScrollPoint(QPointF point);
    void setContentSize(QSizeF size);
    void setZoom(qreal zoom);

private:
    State *_state;
    bool _existsChange;
};
} // namespace Core
#endif // BROWSERSTATE_H
