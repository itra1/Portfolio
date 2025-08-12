QT -= gui

CONFIG += c++14 console
CONFIG -= app_bundle
include($$PWD/../RSAStaticLib/RSAStaticLib.pri);

# You can make your code fail to compile if it uses deprecated APIs.
# In order to do so, uncomment the following line.
#DEFINES += QT_DISABLE_DEPRECATED_BEFORE=0x060000    # disables all the APIs deprecated before Qt 6.0.0

SOURCES += \
        main.cpp

mac {
QMAKE_APPLE_DEVICE_ARCHS = x86_64 arm64
QMAKESPEC = macx-xcode
#QMAKE_MACOSX_DEPLOYMENT_TARGET = 10.15
}

# Default rules for deployment.
qnx: target.path = /tmp/$${TARGET}/bin
else: unix:!android: target.path = /opt/$${TARGET}/bin
!isEmpty(target.path): INSTALLS += target

win32:CONFIG(release, debug|release): LIBS += $$PWD/../RSAStaticLib/build/release/RSAStaticLib.lib
else:win32:CONFIG(debug, debug|release): LIBS += $$PWD/../RSAStaticLib/build/debug/RSAStaticLibd.lib

mac:CONFIG(release, debug|release): LIBS += $$PWD/../RSAStaticLib/build/release/RSAStaticLib.a
else:mac:CONFIG(debug, debug|release): LIBS += $$PWD/../RSAStaticLib/build/debug/RSAStaticLibd.a
