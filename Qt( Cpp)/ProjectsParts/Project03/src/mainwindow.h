#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include "browseraction.h"
#include <CnpApi>
#include "browserstate.h"

namespace Ui
{
    class MainWindow;
}
namespace Core {
class Application;
}

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    explicit MainWindow(Core::Application *app, Core::BrowserState *state, QWidget *parent = nullptr);
    ~MainWindow();

    void SetWindowTitle(QString title);
    void SetState(CnpApi::State *state);
    void SetMaterial(CnpApi::Browser *browser);
    CnpApi::Browser * Browser();
    Ui::MainWindow * Ui();

protected:
    //bool nativeEvent(const QByteArray &eventType, void *message, long *result);

public:
    QUrl GetUrl();
    bool IsIms();
    bool IsBi();

public:
    void Authorization();
    void SetBrowserAction(BrowserAction *handler);
    void LoadUrl(const QUrl &url);

public slots:
    void UrlChangedSlot(const QUrl &url);
    void LoadStart();
    void LoadProgress(int progress);
    void LoadFinish(bool ok);
    void ScrollPointChange(QPointF point);
    void ContentsSizeChanged(QSizeF size);
    void TitleChanged(QString size);

private:
    Core::Application *_app;
    Ui::MainWindow *_ui;
    BrowserAction *_browserActions;
    Core::BrowserState *_browserState;

    QString _token;
    QString _id;
    QString _url;
    QString _materialType;
    QString _subMaterialType;
    CnpApi::State *_state{nullptr};
    CnpApi::Browser *_browser{nullptr};
    bool _waitAuth{false};
};

#endif // MAINWINDOW_H
