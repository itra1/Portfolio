#include "src/application.h"
#include "src/socketclient.h"

#include <QApplication>
#include <QLocale>
#include <QLoggingCategory>
#include <QTranslator>
#include <QWebEngineProfile>
#include <QWebEngineSettings>
#include "src/config/config.h"
#include "src/mainwindow.h"

using namespace Qt::StringLiterals;

int main(int argc, char *argv[])
{
    Config cnf;
    QCoreApplication::setOrganizationName("Cnp Browser");

    QApplication app(argc, argv);
    app.setWindowIcon(QIcon(u":AppLogoColor.png"_s));

    QTranslator translator;
    const QStringList uiLanguages = QLocale::system().uiLanguages();
    for (const QString &locale : uiLanguages) {
        const QString baseName = "Browser_" + QLocale(locale).name();
        if (translator.load(":/i18n/" + baseName)) {
            app.installTranslator(&translator);
            break;
        }
    }
    QLoggingCategory::setFilterRules(u"qt.webenginecontext.debug=false"_s);

    QWebEngineProfile::defaultProfile()->settings()->setAttribute(QWebEngineSettings::PluginsEnabled,
                                                                  true);
    QWebEngineProfile::defaultProfile()
        ->settings()
        ->setAttribute(QWebEngineSettings::DnsPrefetchEnabled, true);
    QWebEngineProfile::defaultProfile()
        ->settings()
        ->setAttribute(QWebEngineSettings::ScreenCaptureEnabled, true);
    QWebEngineProfile::defaultProfile()
        ->settings()->setAttribute(QWebEngineSettings::LocalContentCanAccessRemoteUrls, true);
    QWebEngineProfile::defaultProfile()
        ->settings()->setAttribute(QWebEngineSettings::LocalContentCanAccessFileUrls, true);
    QWebEngineProfile::defaultProfile()
        ->settings()->setAttribute(QWebEngineSettings::JavascriptEnabled, true);
    QWebEngineProfile::defaultProfile()
        ->settings()->setAttribute(QWebEngineSettings::JavascriptCanOpenWindows, true);
    QWebEngineProfile::defaultProfile()
        ->settings()->setAttribute(QWebEngineSettings::JavascriptCanAccessClipboard, true);

    // Browser browser;
    Core::Application application;

    //MainWindow window{};
    //window.show();
    application.ShowWindow();

    return app.exec();
}
