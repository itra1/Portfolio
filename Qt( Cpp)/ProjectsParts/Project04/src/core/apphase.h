#ifndef APPHASE_H
#define APPHASE_H

#include <QObject>

namespace Core{

  class AppPhase : public QObject
  {
    Q_OBJECT

  public:
    enum State{
      None = 0x0,
      Authorization = 0x1,
      Update = 0x2,
      Play = 0x4,
      Settings = 0x8,
      WaitReleases = 0x16,
      Releases = 0x32,
      UpdateCheck = 0x64

    };
    Q_ENUM(State)
  };
}

#endif // APPHASE_H
