cmake_minimum_required(VERSION 3.21.1)

project(AppApi VERSION 1.0 LANGUAGES CXX)

find_package(Qt6 6.2 REQUIRED COMPONENTS Core Network)

add_library(AppApi STATIC
	models/filemodel.h models/filemodel.cpp
	models/releasemodel.h models/releasemodel.cpp
	models/usermodel.h models/usermodel.cpp
	models/userrolemodel.h models/userrolemodel.cpp
	models/basemodel.h models/basemodel.cpp
	api.h api.cpp
	network.h network.cpp
	base/requesttype.h
)

target_link_libraries(AppApi PRIVATE
    Qt6::Core
		Qt6::Network
)
