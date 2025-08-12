#ifndef SINGLETON_H
#define SINGLETON_H

#include <QObject>

namespace Base{

  template <class T>
  class Singleton : public QObject{

  public:

    explicit Singleton(QObject *parent) : QObject(parent){
      initInstance(parent);
    }

    static T *instance(){
      return _instance;
    }

    static void initInstance(QObject *parent = nullptr){
      if(!_instance)
        _instance = new T(parent);
    }

    static void freeInstance(){
      if(_instance){
          delete _instance;
          _instance = nullptr;
        }
    }

  protected:

    static T *_instance = nullptr;

  };
}

#endif // SINGLETON_H
