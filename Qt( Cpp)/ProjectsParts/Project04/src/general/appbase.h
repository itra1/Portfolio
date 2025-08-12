#ifndef APPBASE_H
#define APPBASE_H

#include <QObject>

namespace General{

  class AppBase : public QObject
  {
    Q_OBJECT
  public:
    explicit AppBase(QObject *parent = nullptr);

    virtual void allReleaseDownload();

  signals:

  private:

  };
};

#endif // APPBASE_H
