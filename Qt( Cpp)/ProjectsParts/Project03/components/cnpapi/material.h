#ifndef MATERIAL_H
#define MATERIAL_H

#include <QJsonObject>
#include <QObject>

namespace CnpApi {
struct Material
{
    Material(QJsonObject jObject);

public:
    qint64 Id() const;
    QString Name() const;
    QString Type() const;

protected:
    qint64 _id;
    QString _type;
    QString _name;
};

inline Material::Material(QJsonObject jObject)
{
    _id = jObject.value("id").toInteger();
    _name = jObject.value("name").toString();
    _type = jObject.value("type").toString();
}

inline qint64 Material::Id() const
{
    return _id;
}

inline QString Material::Name() const
{
    return _name;
}

inline QString Material::Type() const
{
    return _type;
}
} // namespace CnpApi
#endif
