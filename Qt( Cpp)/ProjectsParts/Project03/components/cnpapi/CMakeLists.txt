cmake_minimum_required(VERSION 3.21.1)

project(CnpApi VERSION 1.0 LANGUAGES CXX)

find_package(Qt6 6.2 REQUIRED COMPONENTS Core)

add_library(CnpApi STATIC
    CnpApi
    browser.h
    auth.h
    state.h
	material.h
)

target_link_libraries(CnpApi PRIVATE
    Qt6::Core
)
