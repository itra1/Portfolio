#ifndef RELEASEHELPER_H
#define RELEASEHELPER_H

#include <QObject>

namespace Core {

struct ReleaseHelper
{
    static bool MaxVersion(QString v1, QString v2, bool orEquals = false);
};
} // namespace Core
#endif // RELEASEHELPER_H
