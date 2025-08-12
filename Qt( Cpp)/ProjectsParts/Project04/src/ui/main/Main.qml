import QtQuick
import QtQuick.Window
import QtQuick.Controls
import QtQuick.Controls.Material
import AppPhase 1.0
import QtQuick.Layouts 1.12
import "../controls/."
import Config 1.0
import Theme 1.0
import LauncherQml

Item {

    property string mainColor: "#173059"
    property int previousX
    property int previousY
    property int sizeX : 1266
    property int sizeY : 803

    signal moveX(int delta)
    signal moveY(int delta)
    signal minimized();

    id: mainWindow
    visible: true
    width: sizeX
    height: sizeY
    // color: "#00000000"
    // flags: Qt.FramelessWindowHint | Qt.WindowMinimizeButtonHint | Qt.Window

    Connections{
        target: Session

        function onAuthChange(isLogin){
            if(isLogin){
                serverTitle.text = Authorization.serverName();
                var fullname = Session.fullNameQml();
                userName.text = fullname;
            }else{
                serverTitle.text = "";
                userName.text = "";
            }
        }
    }

    FontLoader{
        id: golosRegularFont
        source: "../static/fonts/Golos_Text_Regular.ttf"
    }

    Image {
        id: backGround
        anchors.fill: parent
        source: "../static/img/BackGround.png"
        fillMode: Image.PreserveAspectFit
    }

    Image {
        id: backGroundHeader
        height: 79
        anchors.left: parent.left
        anchors.right: parent.right
        anchors.top: parent.top
        source: "../static/img/HeaderBack.png"
        anchors.rightMargin: 12
        anchors.leftMargin: 8
        anchors.topMargin: 10
        fillMode: Image.PreserveAspectFit

        MouseArea {
                id: headerMouseArea
                width: parent.width
                height: parent.height
                //cursorShape: Qt.DragMoveCursor

                onPressed: {
                    previousX = mouseX
                    previousY = mouseY
                }

                onMouseXChanged: {
                    var dx = mouseX - previousX
                    //mainWindow.setX(mainWindow.x + dx)
                    moveX(dx);
                }

                onMouseYChanged: {
                    var dy = mouseY - previousY
                    //mainWindow.setY(mainWindow.y + dy)
                    moveY(dy);
                }
            }

        Text{
            id: serverTitle
            width: 311
            height: 28
            color: "#162737"
            anchors.left: parent.left
            anchors.top: parent.top
            font.pixelSize: 20
            horizontalAlignment: Text.AlignLeft
            verticalAlignment: Text.AlignVCenter
            font.bold: true
            anchors.leftMargin: 20
            anchors.topMargin: 16
            font.family: golosRegularFont.name
        }

        Text{
            id: userName
            width: 311
            height: 20
            color: "#162737"
            anchors.left: parent.left
            anchors.top: parent.top
            font.pixelSize: 14
            horizontalAlignment: Text.AlignLeft
            verticalAlignment: Text.AlignVCenter
            font.bold: false
            anchors.leftMargin: 20
            anchors.topMargin: 40
            font.family: golosRegularFont.name
        }

        IconeButton{
            id: closeButton
            anchors.right: parent.right
            anchors.top: parent.top
            anchors.rightMargin: 30
            anchors.topMargin: 23
            iconeUrl: "../static/img/CloseIcone.png"
            tooltipString: "Закрыть"
            onOnClick: {
                Manager.quit();
            }
        }

        IconeButton{
            id: minimizeButton
            anchors.right: parent.right
            anchors.top: parent.top
            anchors.rightMargin: closeButton.anchors.rightMargin + 45
            anchors.topMargin: 23
            iconeUrl: "../static/img/MinimizeIcone.png"
            tooltipString: "Свернуть"
            onOnClick: {
                //mainWindow.showMinimized();
                minimized();
            }
        }

        IconeButton{
            id: logoutButton
            anchors.right: parent.right
            anchors.top: parent.top
            anchors.rightMargin: minimizeButton.anchors.rightMargin + 45
            anchors.topMargin: 23
            iconeUrl: "../static/img/LogOut.png"
            visible: Session.isAuth
            tooltipString: "Выйти"
            onOnClick: {
                Session.logout();
            }
        }

        IconeButton{
            id: settingsButton
            anchors.right: parent.right
            anchors.top: parent.top
            anchors.rightMargin: logoutButton.visible
                                 ? logoutButton.anchors.rightMargin + 45
                                 : minimizeButton.anchors.rightMargin + 45
            anchors.topMargin: 23
            iconeUrl: "../static/img/SettingsIcone.png"
            tooltipString: "Настройки"
            onOnClick: {
                App.settingsButtonTouch();
            }
            isSelect: App.isSettings
        }

        IconeButton{
            id: logButton
            anchors.right: parent.right
            anchors.top: parent.top
            anchors.rightMargin: settingsButton.anchors.rightMargin + 45
            anchors.topMargin: 23
            iconeUrl: "../static/img/log.png"
            tooltipString: "Лог лаунчера"
            onOnClick: {
                Manager.openAppLog();
            }
        }

        UpdateButton{
            id: updatebutton
            anchors.right: parent.right
            anchors.rightMargin: logButton.anchors.rightMargin + logButton.width+10
            anchors.top: parent.top
            anchors.topMargin: 23
            visible: UpdaterManager.updateReady
            tooltipString: "До версии " + UpdaterManager.updateVersion
        }
    }

    ReleasesList{
        id: releases
        x: 0
        y: 0
        width: parent.width
        height: parent.height
        visible: App.appState === AppPhase.Releases && !App.isSettings
    }

    SettingsPage{
        id: settingsForm
        anchors.topMargin: 89
        visible: App.isSettings
        anchors.left: parent.left
        anchors.right: parent.right
        anchors.top: parent.top
        anchors.bottom: parent.bottom
        anchors.bottomMargin: 0
        anchors.leftMargin: 20
        anchors.rightMargin: 20
    }

    Auth {
        id: authForm
        x: 0
        y: 0
        width: parent.width
        height: parent.height
        visible: App.appState === AppPhase.Authorization && !App.isSettings
    }

    Image {
        id: logo
        width: 150
        height: 88

        anchors.right: parent.right
        anchors.bottom: parent.bottom
        source: Qt.resolvedUrl(Theme.picturelogo)
        fillMode: Image.PreserveAspectFit
        anchors.bottomMargin: 56
        anchors.rightMargin: 48
    }

    Text{
        x: 1117
        y: 749
        color: "#4b4b4b"
        anchors.right: parent.right
        anchors.bottom: parent.bottom
        font.pixelSize: 12
        anchors.bottomMargin: 39
        anchors.rightMargin: 30
        font.family: golosRegularFont.name
        text: Config.sumLightLabel
        visible: Manager.getIsPresentationMove();
    }

    Text{
        x: 1222
        y: 766
        color: "#4b4b4b"
        anchors.right: parent.right
        anchors.bottom: parent.bottom
        font.pixelSize: 12
        anchors.bottomMargin: 24
        anchors.rightMargin: 30
        font.family: golosRegularFont.name
        text: Manager.appVersion();
    }
}
