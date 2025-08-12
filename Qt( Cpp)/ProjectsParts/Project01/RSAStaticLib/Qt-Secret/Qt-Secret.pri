SUBDIRS += \
	$$PWD/Qt-AES/qaesencryption.pro

DISTFILES += \
	$$PWD/Qt-AES/ccache.pri \

HEADERS += \
	$$PWD/Qt-AES/qaesencryption.h \
	$$PWD/Qt-AES/qtsecret_global.h \
	$$PWD/Qt-RSA/qrsaencryption.h \
	$$PWD/mini-gmp/bigint.h \
	$$PWD/mini-gmp/mini-gmp.h \
	$$PWD/mini-gmp/minigmp_global.h

SOURCES += \
	$$PWD/Qt-AES/qaesencryption.cpp \
	$$PWD/Qt-RSA/qrsaencryption.cpp \
	$$PWD/mini-gmp/bigint.cpp \
	$$PWD/mini-gmp/mini-gmp.c
