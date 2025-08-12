#include "releasehelper.h"
namespace Core {

bool ReleaseHelper::MaxVersion(QString v1, QString v2, bool orEquals)
{
    if (v2 == "")
        return true;
    if (v1 == v2)
        return orEquals;
    try {
        //QStringList v1s = v1.split(QLatin1Char('_'));
        //QStringList v2s = v2.split(QLatin1Char('_'));
        QStringList v1l = v1.split(QLatin1Char('.'));
        QStringList v2l = v2.split(QLatin1Char('.'));

        if (v1l.count() < 2 || v2l.count() < 2)
            return false;

        for (int i = 0; i < v1l.length(); i++) {
            if (i >= v2l.count())
                return v1l[i].toInt() > 0;
            if (v1l[i].toInt() == v2l[i].toInt())
                continue;
            return v1l[i].toInt() > v2l[i].toInt();
        }

        return orEquals;

    } catch (std::exception ex) {
        return false;
    }
    return false;
}

} // namespace Core
