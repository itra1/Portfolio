#ifndef INSTALLSTATE_H
#define INSTALLSTATE_H

#include <QObject>

namespace Core{

    class InstallState : public QObject
    {
        Q_OBJECT

    public:
        enum State {
            None,
            Login,
            InstallReady,
            PlayReady,
            UpdateReady,
            Process
        };

        Q_ENUM(State)
    };
}

#endif // INSTALLSTATE_H
