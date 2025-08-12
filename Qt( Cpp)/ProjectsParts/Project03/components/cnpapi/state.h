#ifndef STATE_H
#define STATE_H

#include <QJsonObject>
#include <QObject>

namespace CnpApi {
struct State
{
    State(QJsonObject jObject);

public:
    qint64 AreaId() const;
    qint64 StatusContentId() const;
    qint64 EpisodeId() const;
    qint64 PresentationId() const;
    double Scale() const;
    QString Url() const;
    double ScrollX() const;
    double ScrollY() const;

    bool UrlUse() const;
    void SetUrlUse(bool NewUrlUse);

private:
    qint64 _areaId;
    qint64 _statusContentId;
    qint64 _episodeId;
    qint64 _presentationId;

    double _scale;
    QString _url;
    double _scrollX;
    double _scrollY;

    bool _urlUse{false};
};

inline State::State(QJsonObject jObject)
{
    _areaId = jObject.value("AreaId").toInteger();
    _statusContentId = jObject.value("StateContentId").toInteger();
    _episodeId = jObject.value("EpisodeId").toInteger();
    _presentationId = jObject.value("PresentationId").toInteger();
    _scale = jObject.value("Scale").toDouble();
    _url = jObject.value("Url").toString();
    _scrollX = jObject.value("ScrollX").toDouble();
    _scrollY = jObject.value("ScrollY").toDouble();
}

inline qint64 State::AreaId() const
{
    return _areaId;
}

inline qint64 State::StatusContentId() const
{
    return _statusContentId;
}

inline qint64 State::EpisodeId() const
{
    return _episodeId;
}

inline qint64 State::PresentationId() const
{
    return _presentationId;
}

inline double State::Scale() const
{
    return _scale;
}

inline QString State::Url() const
{
    return _url;
}

inline double State::ScrollX() const
{
    return _scrollX;
}

inline double State::ScrollY() const
{
    return _scrollY;
}

inline bool State::UrlUse() const
{
    return _urlUse;
}

inline void State::SetUrlUse(bool NewUrlUse)
{
    _urlUse = NewUrlUse;
}

} // namespace CnpApi
#endif
