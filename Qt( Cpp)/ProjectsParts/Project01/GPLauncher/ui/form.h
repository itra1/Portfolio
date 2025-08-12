#ifndef FORM_H
#define FORM_H

#include <QObject>
#include <QQmlApplicationEngine>
#include "core/appPhase.h"

class Form : public QObject
{
  Q_OBJECT
public:
  explicit Form(QQmlApplicationEngine *engine, QObject *parent = nullptr);
  static void initInstance(QQmlApplicationEngine *engine, QObject *parent);
  static void freeInstance();
  static Form *instance();

  Q_PROPERTY(AppPhase::State appPhase READ getAppPhase NOTIFY onAppPhaseChange)
  Q_PROPERTY(double downloadProgress READ getDownloadProgress NOTIFY onDownloadProgres)
  Q_INVOKABLE QString getError();
  Q_INVOKABLE void errorRepeatClickButton();

  AppPhase::State getAppPhase();
  double getDownloadProgress();

signals:
  void onAppPhaseChange();
  void onDownloadProgres();

private slots:
  void phaseChangeSlot();
  void downloadProgresSlot(double value);

private:
  static Form *_instance;
  static QQmlApplicationEngine *m_engine;
  double m_downloadProgress;

};

#endif // FORM_H
