project(pipelib VERSION 0.1 LANGUAGES CXX)

find_package(Qt6 REQUIRED COMPONENTS Core Network)

set(ALL_SRC
	client.h client.cpp
	server.h server.cpp
	actions.h
)
set(ALL_HEADERS
	client.h
	actions.h
)

add_library(pipelib ${ALL_SRC})

target_link_libraries(pipelib
		PRIVATE
				Qt6::Core
				Qt6::Network
)
