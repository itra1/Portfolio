#ifndef BROWSER_H
#define BROWSER_H

#include <QJsonArray>
#include <QJsonObject>
#include <QObject>
#include "auth.h"
#include "material.h"

namespace CnpApi
{
    struct Browser : Material
    {
        Browser(QJsonObject jObject);

    public:
        QString Url() const;
        QString MaterialType() const;
        QString SunType() const;
        CnpApi::Auth *Auth();

    private:
        QString _url;
        QString _materialType;
        QString _subType;
        CnpApi::Auth *_auth{nullptr};
    };

    inline Browser::Browser(QJsonObject jObject)
        : Material(jObject)
    {
        _url = jObject.value("Url").toString();
        _materialType = jObject.value("MaterialType").toString();
        _subType = jObject.value("SubType").toString();

        auto jAuthData = jObject.value("AuthData");
        if ((jAuthData.isString() && jAuthData.toString() == "null" )|| jAuthData.isNull() || jAuthData.toObject().count() <= 0)
            _auth = nullptr;
        else
            _auth = new CnpApi::Auth(jAuthData.toObject());
    }

    inline QString Browser::Url() const
    {
        return _url;
    }
    inline QString Browser::MaterialType() const
    {
        return _materialType;
    }
    inline QString Browser::SunType() const
    {
        return _subType;
    }

    inline CnpApi::Auth *Browser::Auth()
    {
        return _auth;
    }

} // namespace CnpApi
#endif
