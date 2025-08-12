#ifndef ACTION_H
#define ACTION_H

#include <QJsonObject>
#include <QObject>

namespace CnpSocket {
struct Action
{
    Action(QJsonObject jObject);

public:
    QString Name() const;

protected:
    void SetName(QString name);

private:
    QString _name;
};

inline Action::Action(QJsonObject jObject) {}

inline QString Action::Name() const
{
    return _name;
}

inline void Action::SetName(QString name)
{
    _name = name;
}

} // namespace CnpSocket
#endif // ACTION_H
