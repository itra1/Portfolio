#ifndef PLACESTATE_H
#define PLACESTATE_H

#include <QObject>

namespace Core{

    class PlaceState : public QObject
    {
        Q_OBJECT

    public:
        enum State{
            None = 0x0,
            InstallReady = 0x1,
            InstallProcess = 0x2,
            Install = InstallReady | InstallProcess,
            UpdateReady = 0x4,
            UpdateProcess = 0x8,
            Update = UpdateReady | UpdateProcess,
            PlayReady = 0x16,
            PlayProcess = 0x32,
            Play = PlayReady | PlayProcess,
            HashChecking = 0x64,
            Process = InstallProcess | UpdateProcess | PlayProcess | HashChecking
        };
        Q_ENUM(State)
    };
}


#endif // PLACESTATE_H
