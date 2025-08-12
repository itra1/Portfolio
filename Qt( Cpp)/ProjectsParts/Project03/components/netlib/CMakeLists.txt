cmake_minimum_required(VERSION 3.12...3.27 FATAL_ERROR)

project(netlib VERSION 0.1 LANGUAGES CXX)

find_package(Qt6 REQUIRED COMPONENTS Core Network)

set(ALL_SRC
		"network.cpp"
)
set(ALL_HEADERS
		"network.h"
)

add_library(netlib ${ALL_SRC})

target_link_libraries(netlib
		PRIVATE
				Qt6::Core
				Qt6::Network
)
