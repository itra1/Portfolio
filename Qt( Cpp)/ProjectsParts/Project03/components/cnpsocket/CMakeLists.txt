cmake_minimum_required(VERSION 3.21.1)

project(CnpSocket VERSION 1.0 LANGUAGES CXX)

find_package(Qt6 6.2 REQUIRED COMPONENTS Core)

add_library(CnpSocket STATIC
    CnpSocket
    action.h
    materialaction.h
)

target_link_libraries(CnpSocket PRIVATE
    Qt6::Core
)
