qt_add_library(SettingsQml STATIC)

target_link_libraries(SettingsQml PRIVATE Qt6::Quick)

# qt_add_qml_module(SettingsQml
qt_add_qml_module(${CMAKE_PROJECT_NAME}
    URI "SettingsQml"
    OUTPUT_DIRECTORY SettingsQml
    QML_FILES
      BaseSettingsPage.qml
      ServerSettingsPage.qml
      UpdateServerSettingsPage.qml
      CreateServer.qml
      ServersList.qml
)