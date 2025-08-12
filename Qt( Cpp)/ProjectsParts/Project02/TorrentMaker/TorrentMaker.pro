#
# Консольный генератор торрент файлов
#
#

QT += core
TEMPLATE = app
CONFIG += static qt c++11 console

SOURCES += \
        main.cpp \
    generateini.cpp

HEADERS +=

include("../winconfig.pri");

win32:CONFIG(release, debug|release): LIBS += libboost_date_time-vc141-mt-1_64.lib
else:win32:CONFIG(debug, debug|release): LIBS += libboost_date_time-vc141-mt-gd-1_64.lib
win32:CONFIG(release, debug|release): LIBS += libboost_atomic-vc141-mt-1_64.lib
else:win32:CONFIG(debug, debug|release): LIBS += libboost_atomic-vc141-mt-gd-1_64.lib

