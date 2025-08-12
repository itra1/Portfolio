QT -= gui

TEMPLATE = lib
CONFIG += staticlib

CONFIG += c++14

include($$PWD/RSAStaticLib.pri);
include($$PWD/Qt-Secret/Qt-Secret.pri);

# You can make your code fail to compile if it uses deprecated APIs.
# In order to do so, uncomment the following line.
#DEFINES += QT_DISABLE_DEPRECATED_BEFORE=0x060000    # disables all the APIs deprecated before Qt 6.0.0

HEADERS += \
		rsastaticlib.h

SOURCES += \
                rsastaticlib.cpp

CONFIG(release, debug|release): {
		DESTDIR="$$PWD/build/release"
		TARGET = "RSAStaticLib"
} else {
		DESTDIR="$$PWD/build/debug"
		TARGET = "RSAStaticLibd"
}

mac {
QMAKE_APPLE_DEVICE_ARCHS = x86_64 arm64
QMAKESPEC = macx-xcode
QMAKE_MACOSX_DEPLOYMENT_TARGET = 10.15
}

# Default rules for deployment.
unix {
    target.path = $$[QT_INSTALL_PLUGINS]/generic
}
!isEmpty(target.path): INSTALLS += target


