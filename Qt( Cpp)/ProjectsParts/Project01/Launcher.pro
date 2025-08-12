TEMPLATE = subdirs
CONFIG += ordered

CONFIG+=sdk_no_version_check


SUBDIRS += \
					RSAStaticLib \
					RSACrypter \
					GPLauncher

mac {
#QMAKE_APPLE_DEVICE_ARCHS = x86_64 arm64
QMAKE_APPLE_DEVICE_ARCHS = x86_64 arm64
QMAKESPEC = macx-xcode
QMAKE_MACOSX_DEPLOYMENT_TARGET = 10.15
}

RSAStaticLib.file = RSAStaticLib/RSAStaticLib.pro
RSACrypter.file = RSACrypter/RSACrypter.pro
GPLauncher.file = GPLauncher/GPLauncher.pro

win32:DEFINES += WIN_OS
macx|macos:DEFINES += MAC_OS
