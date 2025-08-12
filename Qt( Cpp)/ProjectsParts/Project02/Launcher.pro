# Компановка:
# msvc-14.1 (64bit)
# Qt 5.12
# boost 1.64.0
# crypto: openssl-1.0.2l
# libtorrent 1.1.12

QT += core quick gui svg webengine # #network #webengine widgets xml quickcontrols2
CONFIG += c++11
CONFIG += staticqt windows #staticlib

CONFIG += qtquickcompiler

#include("../winconfig.pri");

include("version.pri");

RC_ICONS = gui/img/logo-white.ico

#DEFINES += QT_DEPRECATED_WARNINGS
#DEFINES += QT_DISABLE_DEPRECATED_BEFORE=0x060000    # disables all the APIs deprecated before Qt 6.0.0

SOURCES += \
        main.cpp

RESOURCES +=

TRANSLATIONS += gui/translations/launcher_ru_RU.ts \
                gui/translations/launcher_en_EN.ts

lupdate_only{
SOURCES = \
    $$PWD/gui/forms/FooterInstallBlock.qml \
    $$PWD/gui/forms/FooterPlayBlock.qml \
    $$PWD/gui/forms/FooterProcessBlock.qml \
    $$PWD/gui/forms/FooterUpdateBlock.qml \
    $$PWD/gui/forms/HeaderWindowControl.qml \
    $$PWD/gui/forms/License.qml \
    $$PWD/gui/forms/main.qml \
    $$PWD/gui/forms/Settings.qml
}


#QT_DEBUG_PLUGINS = 1

# Additional import path used to resolve QML modules in Qt Creator's code model
QML_IMPORT_PATH =

# Additional import path used to resolve QML modules just for Qt Quick Designer
QML_DESIGNER_IMPORT_PATH =

# Default rules for deployment.
qnx: target.path = /tmp/$${TARGET}/bin
else: unix:!android: target.path = /opt/$${TARGET}/bin
!isEmpty(target.path): INSTALLS += target

include("core/core.pri");
include("base/base.pri");
include("gui/gui.pri");

HEADERS +=
