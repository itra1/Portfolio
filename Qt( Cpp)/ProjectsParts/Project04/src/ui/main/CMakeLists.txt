
# qt_add_qml_module(MainQml
qt_add_qml_module(${CMAKE_PROJECT_NAME}
    URI "MainQml"
    QML_FILES
      Base.qml
      Main.qml
      Run.qml
      Auth.qml
      Update.qml
      ReleasesList.qml
      ReleaseItem.qml
      ServiceSettings.qml
      SettingsPage.qml
      Theme.qml
      Config.qml
)