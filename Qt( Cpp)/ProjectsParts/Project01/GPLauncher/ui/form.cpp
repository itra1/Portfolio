#include <QQmlApplicationEngine>
#include <QQmlContext>
#include "form.h"
#include "core/appPhase.h"
#include "core/application.h"

Form *Form::_instance = nullptr;
QQmlApplicationEngine *Form::m_engine = nullptr;

Form::Form(QQmlApplicationEngine *engine, QObject *parent) : QObject(parent)
{
  m_engine = engine;
  qmlRegisterType<AppPhase>("AppPhase",1,0,"AppPhase");
  engine->rootContext()->setContextProperty("App",Application::instance());
  engine->rootContext()->setContextProperty("Manager",this);
  const QUrl url(QStringLiteral("qrc:/qml/Main.qml"));
//  QObject::connect(&engine, &QQmlApplicationEngine::objectCreated,
//                   &app, [url](QObject *obj, const QUrl &objUrl) {
//    if (!obj && url == objUrl)
//      QCoreApplication::exit(-1);
//  }, Qt::QueuedConnection);
  engine->load(url);

  connect(Application::instance(),SIGNAL(onChangePhase()),SLOT(phaseChangeSlot()));
  connect(Application::instance(),SIGNAL(onDownloadProgress(double)),SLOT(downloadProgresSlot(double)));
}

void Form::initInstance(QQmlApplicationEngine *engine, QObject *parent)
{
    if(!_instance)
        _instance = new Form(engine,parent);
}

void Form::freeInstance()
{
    if (_instance) {
        delete _instance;
        _instance = nullptr;
    }
}

Form *Form::instance()
{
  return _instance;
}

QString Form::getError()
{
  qDebug() << Application::instance()->getError();
  return Application::instance()->getError();
}

void Form::errorRepeatClickButton()
{
  Application::instance()->repeatButton();
}

AppPhase::State Form::getAppPhase()
{
  return Application::instance()->getPhase();
}

double Form::getDownloadProgress()
{
  return m_downloadProgress;
}

void Form::phaseChangeSlot()
{
  emit onAppPhaseChange();
}

void Form::downloadProgresSlot(double value)
{
  m_downloadProgress = value;
  emit onDownloadProgres();
}
