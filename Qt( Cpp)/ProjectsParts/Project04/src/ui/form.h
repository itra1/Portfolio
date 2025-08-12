#ifndef FORM_H
#define FORM_H

#include <QList>
#include <QObject>
#include <QQmlApplicationEngine>
#include "../core/apphase.h"

namespace UI {

class Form : public QObject
{
    Q_OBJECT
public:
    explicit Form(QQmlApplicationEngine *engine, QObject *parent = nullptr);
    static void initInstance(QQmlApplicationEngine *engine, QObject *parent);
    static void freeInstance();
    static Form *instance();

    void setInfoText(QString newText);
    QString getInfoText();
    QString getloginErrorText();
    bool isloginError();
    bool isPlayReady();
    bool isAuthProcess();
    Core::AppPhase::State appState();
    QUrl getTheme();
    Q_INVOKABLE bool getIsPresentationMove();
    Q_INVOKABLE void clearBrowserCache();
    Q_INVOKABLE void clearAllCache();
    Q_INVOKABLE void quit();
    Q_INVOKABLE QString appVersion();
    Q_INVOKABLE void openAppLog();
    Q_INVOKABLE void openClientLog();

    Q_PROPERTY(QString infoText READ getInfoText NOTIFY onInfoText);
    Q_PROPERTY(bool isloginError READ isloginError NOTIFY onloginErrorChange);
    Q_PROPERTY(QString loginErrorText READ getloginErrorText NOTIFY onloginErrorChange);
    Q_PROPERTY(bool isPlayReady READ isPlayReady NOTIFY onSetPlayReady);
    Q_PROPERTY(bool isAuthProcess READ isAuthProcess NOTIFY onAuthProcess)

signals:
    void onInfoText();
    void onloginErrorChange();
    void onSetPlayReady();
    void onReleaseChange();
    void onAuthProcess();
    void onDownloadError(QString error);

public slots:
    void loginError(QString error);
    void authProcess();

private:
    static Form *_instance;
    static QQmlApplicationEngine *_qmlEngine;
    QString _infoText{""};
    QString _loginError{""};
    QString _combinationKey{""};
};
} // namespace UI

#endif // FORM_H
