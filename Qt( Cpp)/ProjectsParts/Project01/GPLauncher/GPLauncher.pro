QT += core quick qml network

CONFIG += c++14

LIBS += $$quote(-L$$PWD/lib)

include("ui/ui.pri");
include("localization/localization.pri");
include("version.pri");
include("core/core.pri");
include("config/config.pri");
include("helpers/helpers.pri");
#include($$PWD/../Qt-Secret/src/Qt-Secret.pri);

TARGET = GPLauncher

PRODUCT_BUNDLE_IDENTIFIER = com.garillaPoker.gp.launcher
QMAKE_TARGET_BUNDLE_PREFIX = com.garillaPoker.gp
QMAKE_BUNDLE = launcher

mac {
QMAKE_APPLE_DEVICE_ARCHS = x86_64 arm64
QMAKESPEC = macx-xcode
QMAKE_MACOSX_DEPLOYMENT_TARGET = 10.15
}

# You can make your code fail to compile if it uses deprecated APIs.
# In order to do so, uncomment the following line.
DEFINES += QT_DISABLE_DEPRECATED_BEFORE=0x060000    # disables all the APIs deprecated before Qt 6.0.0

DEFINES += API_V1

#QMAKE_EXTRA_TARGETS += before_build makefilehook

#makefilehook.target = $(MAKEFILE)
#makefilehook.depends = .beforebuild

#PRE_TARGETDEPS += .beforebuild

#before_build.target = .beforebuild
#before_build.depends = FORCE
#before_build.commands = chcp 1251


#win32-msvc* {
#CONFIG += embed_manifest_exe
#QMAKE_LFLAGS_WINDOWS += $$quote( /MANIFESTUAC:\"level=\'requireAdministrator\' uiAccess=\'true\'\" )
#}


win32 {
		CONFIG -= embed_manifest_exe
		RC_FILE = GPLauncher.rc
}

SOURCES += \
				main.cpp

#CONFIG += lrelease
#CONFIG += embed_translations

# Additional import path used to resolve QML modules in Qt Creator's code model
macx {
QML_IMPORT_PATH += "/Users/it_ra/Qt/5.15.2/clang_static_64/include"
ICON = ui/image/GPIcone.icns
}
win23 {
QML_IMPORT_PATH += "f:/Qt/5.15.2/msvc2019_64_static/include"
RC_ICONS = ui/image/LauncherLogo.ico
}


# Additional import path used to resolve QML modules just for Qt Quick Designer
#QML_DESIGNER_IMPORT_PATH =

# Default rules for deployment.
qnx: target.path = /tmp/$${TARGET}/bin
else: unix:!android: target.path = /opt/$${TARGET}/bin
!isEmpty(target.path): INSTALLS += target

INCLUDEPATH += $$PWD/include \
                $$PWD/include/zlib\
                $$PWD/include/quazip\
                $$PWD/include/rsa\

win32: LIBS += $$PWD/lib/zlib.lib
win32:CONFIG(debug, debug|release): LIBS += $$PWD/lib/zlibd.lib

win32:CONFIG(release, debug|release): LIBS += $$PWD/lib/quazip1-qt5.lib
else:win32:CONFIG(debug, debug|release): LIBS += $$PWD/lib/quazip1-qt5d.lib

win32:CONFIG(release, debug|release): LIBS += $$PWD/lib/RSAStaticLib.lib
else:win32:CONFIG(debug, debug|release): LIBS += $$PWD/lib/RSAStaticLibd.lib

# macOS

macx|macos:CONFIG(release, debug|release): LIBS += $$PWD/lib/RSAStaticLib.a
else:macx|macos:CONFIG(debug, debug|release): LIBS += $$PWD/lib/RSAStaticLibd.a

mac:CONFIG(release, debug|release): LIBS += $$PWD/lib/RSAStaticLib.a
mac: LIBS += $$PWD/lib/libcrypto.a
mac: LIBS += $$PWD/lib/libssl.a
mac: LIBS += $$PWD/lib/libz.a
mac: LIBS += $$PWD/lib/libquazip1-qt5.a

win32 {
    HEADERS += \
     \	GPLauncher.rc
 }

SUBDIRS += \

DISTFILES += \
