cmake_minimum_required(VERSION 3.12...3.27 FATAL_ERROR)

option(RECORD_FILE_LOG "Record log file" ON)

# Версия приложения
include(${CMAKE_CURRENT_SOURCE_DIR}/appversion)

set(PRJ_NAME "Browser")
set(PRJ_COPYRITE "Copyright © 2024 CNP")
add_compile_definitions(PRJ_NAME=\"${PRJ_NAME}\")
add_compile_definitions(CMAKE_GENERATOR_PLATFORM=x64)

project(${PRJ_NAME} VERSION ${PRJ_VERSION} LANGUAGES CXX)

enable_language("RC")
add_compile_definitions(APP_VERSION=\"${PRJ_VERSION}\")

set(CMAKE_AUTOUIC ON)
set(CMAKE_AUTOMOC ON)
set(CMAKE_AUTORCC ON)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

find_package(QT NAMES Qt6 Qt5 REQUIRED COMPONENTS Core Gui Widgets WebEngineWidgets LinguistTools Core5Compat)
find_package(Qt${QT_VERSION_MAJOR} REQUIRED COMPONENTS Core Gui Widgets WebEngineWidgets LinguistTools Core5Compat)
include(GNUInstallDirs)
find_package(OpenSSL)

set(TS_FILES Browser_ru_RU.ts)

set(PROJECT_SOURCES WIN32
				main.cpp
				${CMAKE_CURRENT_BINARY_DIR}/version.rc
				src/config/configs.qrc
				src/config/config.h
				src/config/configItem.h
				src/config/config.cpp
				src/config/configKeys.h
				src/socketclient.h src/socketclient.cpp
				src/application.h src/application.cpp
				src/customtypes.h
				src/socket/packs/windowMaterialAction.h
				src/socket/packs/packbase.h
				src/socket/packs/actionpack.h
				src/jsactions.h
				src/mainwindow.h src/mainwindow.cpp src/mainwindow.ui
				src/logger.h src/logger.cpp
				src/browseraction.h src/browseraction.cpp
				src/pipeactions.h
				src/pipehandler.h src/pipehandler.cpp
				src/browserstate.h src/browserstate.cpp
)

add_executable(${PRJ_NAME}
	${PROJECT_SOURCES}
	${TS_FILES}
)

include_directories(sioclient)
add_subdirectory(sioclient)
include_directories(components/netlib)
add_subdirectory(components/netlib)
include_directories(components/qtsystemlib)
add_subdirectory(components/qtsystemlib)
include_directories(components/cnpapi)
add_subdirectory(components/cnpapi)
include_directories(components/cnpsocket)
add_subdirectory(components/cnpsocket)
include_directories(components/pipe)
add_subdirectory(components/pipe)

qt_create_translation(QM_FILES ${CMAKE_SOURCE_DIR} ${TS_FILES})

set_target_properties(${PRJ_NAME} PROPERTIES
    WIN32_EXECUTABLE TRUE
)
target_compile_definitions(${PRJ_NAME} PUBLIC CMAKE_GENERATOR_PLATFORM=x64)

target_link_libraries(${PRJ_NAME} PUBLIC
	Qt${QT_VERSION_MAJOR}::Core
	Qt${QT_VERSION_MAJOR}::Gui
	Qt${QT_VERSION_MAJOR}::Widgets
	Qt${QT_VERSION_MAJOR}::WebEngineWidgets
	Qt${QT_VERSION_MAJOR}::Core5Compat
	sioclient_tls
	netlib
	QtSystemLib
	CnpApi
	CnpSocket
	pipelib
)

# Resources:
set(resource_images
		"res/images/AppLogoColor.png"
)

# Set record log
if(RECORD_FILE_LOG)
	add_definitions(-DLOG_FILE)
endif()

qt_add_resources(${PRJ_NAME} "resources"
		PREFIX
				"/"
		BASE
				"res/images"
		FILES
				${resource_images}
)

configure_file(
				${CMAKE_CURRENT_SOURCE_DIR}/version.rc.in
				${CMAKE_CURRENT_BINARY_DIR}/version.rc
				@ONLY)

install(TARGETS ${PRJ_NAME}
    BUNDLE DESTINATION .
    LIBRARY DESTINATION ${CMAKE_INSTALL_LIBDIR}
    RUNTIME DESTINATION ${CMAKE_INSTALL_BINDIR}
)

qt_finalize_executable(${PRJ_NAME})
