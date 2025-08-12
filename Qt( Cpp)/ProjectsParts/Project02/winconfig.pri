# Внимание!!! Файл общий для всех подпроектов

LIBS += $$quote(-L$$PWD/lib)

INCLUDEPATH += $$PWD/include \
            $$PWD/include/zlib\
            $$PWD/config
DEPENDPATH += $$PWD/include

DEFINES += BOOST_ALL_NO_LIB
DEFINES += BOOST_ASIO_ENABLE_CANCELIO
DEFINES += BOOST_ASIO_HASH_MAP_BUCKETS=1021
DEFINES += BOOST_CHRONO_STATIC_LINK=1
DEFINES += BOOST_EXCEPTION_DISABLE
#DEFINES += BOOST_MULTI_INDEX_DISABLE_SERIALIZATION
# Указывать при компиляции libtorrent
DEFINES += BOOST_NO_DEPRECATED
#DEFINES += BOOST_SYSTEM_NO_DEPRECATED
DEFINES += BOOST_SYSTEM_STATIC_LINK=1
win32:CONFIG(release, debug|release): DEFINES += NDEBUG
win32:CONFIG(debug, debug|release): DEFINES += TORRENT_PRODUCTION_ASSERTS
#DEFINES += TORRENT_BUILDING_LIBRARY
DEFINES += TORRENT_USE_I2P=1
DEFINES += TORRENT_USE_OPENSSL
DEFINES += TORRENT_DISABLE_GEO_IP
DEFINES += TORRENT_DISABLE_RESOLVE_COUNTRIES
DEFINES += TORRENT_NO_DEPRECATE
DEFINES += UNICODE
DEFINES += WIN32
DEFINES += WIN32_LEAN_AND_MEAN
#DEFINES += _CRT_SECURE_NO_DEPRECATE
DEFINES += _FILE_OFFSET_BITS=64
#DEFINES += _SCL_SECURE_NO_DEPRECATE
DEFINES += _UNICODE
DEFINES += _WIN32
DEFINES += _WIN32_WINNT=0x0600
DEFINES += __USE_W32_SOCKETS

win32:CONFIG(release, debug|release): LIBS += libboost_system-vc141-mt-1_64.lib
else:win32:CONFIG(debug, debug|release): LIBS += libboost_system-vc141-mt-gd-1_64.lib

win32:CONFIG(release, debug|release): LIBS += libeay32MT.lib
else:win32:CONFIG(debug, debug|release): LIBS += libeay32MTd.lib

win32:CONFIG(release, debug|release): LIBS += ssleay32MT.lib
else:win32:CONFIG(debug, debug|release): LIBS += ssleay32MTd.lib

win32:CONFIG(release, debug|release): LIBS += wbemuuid.lib
else:win32:CONFIG(debug, debug|release): LIBS += wbemuuid.lib

win32: LIBS += iphlpapi.Lib
win32: LIBS += AdvAPI32.Lib
win32: LIBS += Crypt32.Lib
win32: LIBS += User32.Lib
win32: LIBS += Gdi32.Lib
win32: LIBS += Ole32.Lib

win32:CONFIG(debug, debug|release): LIBS += dbghelp.lib

win32:CONFIG(release, debug|release): LIBS += libtorrent.lib
else:win32:CONFIG(debug, debug|release): LIBS += libtorrentd.lib
