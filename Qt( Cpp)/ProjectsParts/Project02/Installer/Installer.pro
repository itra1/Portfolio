#-------------------------------------------------
#
# Project created by QtCreator 2019-02-27T21:48:56
#
#-------------------------------------------------

QT       += core gui network
CONFIG += c++11
CONFIG += staticqt windows #staticlib

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

TARGET = Installer
TEMPLATE = app

RC_ICONS = gui/image/logo-white.ico

#win32-msvc* {
#CONFIG += embed_manifest_exe
#QMAKE_LFLAGS_WINDOWS += $$quote( /MANIFESTUAC:\"level=\'requireAdministrator\' uiAccess=\'true\'\" )
#}

win32 {
    CONFIG -= embed_manifest_exe
    RC_FILE = Installer.rc
}

include("../winconfig.pri");
include("core/core.pri");
include("gui/gui.pri");
include("base/base.pri");
include("version.pri");

TRANSLATIONS += gui/translations/installer_ru_RU.ts \
                gui/translations/installer_en_EN.ts

# The following define makes your compiler emit warnings if you use
# any feature of Qt which has been marked as deprecated (the exact warnings
# depend on your compiler). Please consult the documentation of the
# deprecated API in order to know how to port your code away from it.
#DEFINES += QT_DEPRECATED_WARNINGS

# You can also make your code fail to compile if you use deprecated APIs.
# In order to do so, uncomment the following line.
# You can also select to disable deprecated APIs only up to a certain version of Qt.
#DEFINES += QT_DISABLE_DEPRECATED_BEFORE=0x060000    # disables all the APIs deprecated before Qt 6.0.0

CONFIG += c++11

SOURCES += \
        main.cpp

HEADERS += \

FORMS += \

QMAKE_CXX_FLAGS += /Zc:wchar_t-

# Default rules for deployment.
qnx: target.path = /tmp/$${TARGET}/bin
else: unix:!android: target.path = /opt/$${TARGET}/bin
!isEmpty(target.path): INSTALLS += target
