qt_add_library(ControlsQml STATIC)

target_link_libraries(ControlsQml PRIVATE Qt6::Quick)

# qt_add_qml_module(ControlsQml
qt_add_qml_module(${CMAKE_PROJECT_NAME}
    URI "ControlsQml"
    OUTPUT_DIRECTORY ControlsQml
    QML_FILES
      IconeButton.qml
      Header.qml
      CustomCheckBox.qml
      SettingsInput.qml
      SettingsToggle.qml
      SettingsNavigationButton.qml
      Tooltip.qml
      SettingsBorderInput.qml
      SettingsVectorInput.qml
      ButtonCheckbox.qml
      ServerButton.qml
      StandartButton.qml
      StandartInput.qml
      ErrorToggle.qml
      SettingsDropdown.qml
)