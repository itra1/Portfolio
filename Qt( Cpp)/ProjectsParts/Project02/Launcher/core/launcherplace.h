#ifndef LAUNCHERPLACE_H
#define LAUNCHERPLACE_H

#include <QObject>
#include "place.h"

namespace Core{

    class LauncherPlace : public Place
    {
        Q_OBJECT
    public:
        explicit LauncherPlace(QObject *parent = nullptr);
        bool isGame() override;
        void checkState() override;

        void update() override;

    signals:

    public slots:

    private:
        SourceInfo* getActualVersion();
        QString getSourceUrl() override;

    };
}

#endif // LAUNCHERPLACE_H
