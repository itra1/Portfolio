#ifndef UPDATEPHASE_H
#define UPDATEPHASE_H


#include <QObject>


  class UpdatePhase : public QObject
  {
    Q_OBJECT

  public:
    enum State{
      None = 0x0,
      Launcher = 0x1,
      Game = 0x2
    };
    Q_ENUM(State)
  };


#endif // UPDATEPHASE_H
