qt_add_library(StaticContent STATIC)

target_link_libraries(StaticContent PRIVATE)

# qt_add_qml_module(StaticContent
qt_add_qml_module(${CMAKE_PROJECT_NAME}
    URI "StaticContent"
    OUTPUT_DIRECTORY StaticContent
    RESOURCES
      img/BackGround.png
      img/BackGroundMini.png
      img/CheckError.png
      img/CheckNone.png
      img/CheckOk.png
      img/CloseIcone.png
      img/DownloadButton.png
      img/FolderButton.png
      img/HeaderBack.png
      img/InstallButton.png
      img/log.png
      img/logClient.png
      img/Logo.png
      img/LogOut.png
      img/Menu.png
      img/MinimizeIcone.png
      img/PlayButton.png
      img/RcLogo.ico
      img/RemoveButton.png
      img/ScreenPreview.png
      img/SettingsBack.png
      img/SettingsIcone.png
      img/ShareButton.png
      img/StarClear.png
      img/StarFill.png
      fonts/Golos_Text_Black.ttf
      fonts/Golos_Text_Bold.ttf
      fonts/Golos_Text_DemiBold.ttf
      fonts/Golos_Text_Medium.ttf
      fonts/Golos_Text_Regular.ttf
)