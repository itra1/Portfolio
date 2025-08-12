cmake_minimum_required(VERSION 3.20)

PROJECT(QtSystemLib VERSION 1.0.0 )

set(ALL_SRC
	src/iohelper.h src/iohelper.cpp
	src/filehelper.h src/filehelper.cpp
)

add_library(QtSystemLib ${ALL_SRC})

target_link_libraries(QtSystemLib PUBLIC Qt${QT_VERSION_MAJOR}::Core)
