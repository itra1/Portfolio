cmake_minimum_required(VERSION 3.20)

# Версия приложения
include(${CMAKE_CURRENT_SOURCE_DIR}/appversion)

set(PRJ_NAME "Launcher")
add_compile_definitions(PRJ_NAME=\"${PRJ_NAME}\")

#Темы
#cnp Стандартная схема 
#luk Лукоил
set(CNP_THEME "cnp")
set(LUK_THEME "luk")
set(APP_THEME ${CNP_THEME})

if(APP_THEME STREQUAL ${LUK_THEME})
	add_compile_definitions(-DLUK_THEME)
	set(ISO_THEME "/src/ui/themes/lukoil/img/RcLogo.ico")
else()
	add_compile_definitions(-DCNP_THEME)
	set(ISO_THEME "/src/ui/themes/cnp/img/RcLogo.ico")
endif()

project(${PRJ_NAME} VERSION ${PRJ_VERSION} LANGUAGES CXX)

set(CMAKE_AUTOUIC ON)
set(CMAKE_AUTOMOC ON)
set(CMAKE_AUTORCC ON)
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(BUILD_SHARED_LIBS OFF)

find_package(Qt6 REQUIRED COMPONENTS Core Quick Gui Concurrent)
qt_standard_project_setup(REQUIRES 6.5)

# Подключаем дополнительные модулии
include(${CMAKE_CURRENT_SOURCE_DIR}/extendmodules)
# Подключаем свои модули
include(${CMAKE_CURRENT_SOURCE_DIR}/appmodules)
# Подключение утилит
# include(${CMAKE_CURRENT_SOURCE_DIR}/apputils)

enable_language("RC")

add_compile_definitions(PROJECT_NAME=\"${PRJ_NAME}\")

qt_add_executable(${PRJ_NAME} WIN32
		src/main.cpp
		${CMAKE_CURRENT_BINARY_DIR}/version.rc
		${QuaZip_HEADERS}
		${AppApi_HEADERS}
		src/config/configs.qrc
		src/config/config.h
		src/config/configItem.h
		src/config/config.cpp
		src/config/configKeys.h
		src/base/Singleton.h
		src/general/tag.h
		src/general/appbase.h src/general/appbase.cpp
		src/general/authorization.h src/general/authorization.cpp
		src/general/settingsstorage.h src/general/settingsstorage.cpp
		src/network/network.h src/network/network.cpp
		src/ui/form.h src/ui/form.cpp
		src/base/ibase.h
		src/core/apphase.h
		src/core/users/user.h src/core/users/user.cpp
		src/core/users/userrole.h src/core/users/userrole.cpp
		src/core/wall/clientlog.h src/core/wall/clientlog.cpp
		src/core/api.h src/core/api.cpp
		src/core/application.h src/core/application.cpp
		src/core/configcontroller.h src/core/configcontroller.cpp
		src/core/errorcontroller.h src/core/errorcontroller.cpp
		src/core/logger.h src/core/logger.cpp
		src/core/releasemanager.h src/core/releasemanager.cpp
		src/core/serversmanager.h src/core/serversmanager.cpp
		src/core/session.h src/core/session.cpp
		src/core/settings.h src/core/settings.cpp
		src/core/updater.h src/core/updater.cpp
		src/core/structs/apibase.h
		src/core/structs/renderStreamingConnectType.h
		src/core/structs/server.h src/core/structs/server.cpp
		src/core/settings/settingsitem.h src/core/settings/settingsitem.cpp
		src/core/settings/settingsitemsfactory.h src/core/settings/settingsitemsfactory.cpp
		src/core/settings/settingsitemtoggle.h src/core/settings/settingsitemtoggle.cpp
		src/core/settings/commonsettings.h src/core/settings/commonsettings.cpp
		src/core/settings/settingsiteminputint.h src/core/settings/settingsiteminputint.cpp
		src/core/settings/settingsitemfilepath.h src/core/settings/settingsitemfilepath.cpp
		src/core/settings/settingsbase.h src/core/settings/settingsbase.cpp
		src/core/settingsjsoncontroller.h src/core/settingsjsoncontroller.cpp
		src/core/settings/settingsiteminputint.h src/core/settings/settingsiteminputint.cpp
		src/core/settings/settingsiteminputstring.h src/core/settings/settingsiteminputstring.cpp
		src/core/settings/settingsiteminputpassword.h src/core/settings/settingsiteminputpassword.cpp
		src/core/settings/settingsitemtoggleregedit.h src/core/settings/settingsitemtoggleregedit.cpp
		src/core/settings/settingsiteminputfloat.h src/core/settings/settingsiteminputfloat.cpp
		src/core/releases/fileinfo.h
		src/core/releases/releasestate.h
		src/core/releases/releasetag.h src/core/releases/releasetag.cpp
		src/core/releases/release.h src/core/releases/release.cpp
		src/core/releases/videowall.h src/core/releases/videowall.cpp
		src/core/releases/launcher.h src/core/releases/launcher.cpp
		src/core/releases/streamingassets.h src/core/releases/streamingassets.cpp
		src/core/releases/cross.h src/core/releases/cross.cpp
		src/core/releases/runrelease.h src/core/releases/runrelease.cpp
		src/core/releases/browser.h src/core/releases/browser.cpp
		src/core/releaseloader.h src/core/releaseloader.cpp
		src/core/browsermanager.h src/core/browsermanager.cpp
		src/core/helpers/releasehelper.h src/core/helpers/releasehelper.cpp
)

include(${CMAKE_CURRENT_SOURCE_DIR}/appqml)

set_target_properties(${PRJ_NAME} PROPERTIES
		MACOSX_BUNDLE_BUNDLE_VERSION ${PROJECT_VERSION}
		MACOSX_BUNDLE_SHORT_VERSION_STRING ${PROJECT_VERSION_MAJOR}.${PROJECT_VERSION_MINOR}
		MACOSX_BUNDLE TRUE
		WIN32_EXECUTABLE ON
)

target_link_libraries(${PRJ_NAME}
		PRIVATE
				Qt6::Core
				Qt6::Gui
				Qt6::Qml
				Qt6::Quick
				Qt6::Concurrent
		PUBLIC
				QuaZip
				AppApi
				QtSystemLib
)

configure_file(
				${CMAKE_CURRENT_SOURCE_DIR}/version.rc.in
				${CMAKE_CURRENT_BINARY_DIR}/version.rc
				@ONLY)

include(GNUInstallDirs)
install(TARGETS ${PRJ_NAME}
		BUNDLE DESTINATION .
		LIBRARY DESTINATION ${CMAKE_INSTALL_LIBDIR}
		RUNTIME DESTINATION ${CMAKE_INSTALL_BINDIR}
)
