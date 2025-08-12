#ifndef APPPHASE_H
#define APPPHASE_H

#include <QObject>


  class AppPhase : public QObject
  {
    Q_OBJECT

  public:
    enum State{
      Connect = 0x0,
      Download = 0x1,
      Error = 0x2,
      Play = 0x4
    };
    Q_ENUM(State)
  };


#endif // APPHASE_H
