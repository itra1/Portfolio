#include "errorcontroller.h"
#include <QVariantList>
#include "../general/authorization.h"
#include "releasemanager.h"
#include "session.h"

namespace Core{

  ErrorController* ErrorController::_instance = nullptr;

  ErrorController::ErrorController(QObject *parent) : QObject(parent)
  {

  }

  void ErrorController::initInstance(QObject *parent)
  {
    if(!_instance)
      _instance = new ErrorController(parent);
  }

  void ErrorController::freeInstance()
  {
    if (_instance) {
        delete _instance;
        _instance = nullptr;
      }
  }

  ErrorController *ErrorController::instance()
  {
    return _instance;
  }

  QVariantList ErrorController::getErrorList()
  {
    QVariantList list;

    auto releaseError = Core::ReleaseManager::instance()->getErrorMessage();
		auto user = Core::Session::instance()->user();

		if(releaseError.length() > 0)
      list.append(QVariant::fromValue(releaseError));

    if(user == nullptr){
        list.append(QVariant::fromValue(QString{"Не удалось получить данные пользователя"}));
      }else{
        auto userError = user->getErrorList();

        for(auto err : userError){
            list.append(QVariant::fromValue(QString{err}));
          }
      }

    return list;

  }

}
