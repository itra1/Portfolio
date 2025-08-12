#ifndef BROWSER_H
#define BROWSER_H

#include "release.h"

namespace Core {
class Browser : public Release
{
public:
    Browser();
    Browser(QJsonObject jObject);

    const QString installPath() const override;
    const QString exePath() const override;
};
} // namespace Core

#endif // BROWSER_H
