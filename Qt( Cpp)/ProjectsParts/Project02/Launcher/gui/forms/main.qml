import QtQuick 2.12
import QtQuick.Window 2.12
import QtQuick.Controls 2.4
import QtQuick.Controls.Material 2.3
import QtGraphicalEffects 1.0
import QtWebEngine 1.8
import PlaceState 1.0

Window {
    id: mainWindow
    visible: true
    width: 1000
    height: 700
    minimumWidth: 200
    minimumHeight: 200
    maximumHeight: 700
    maximumWidth: 1000
    color: "transparent"
    flags: Qt.FramelessWindowHint |
           Qt.WindowMinimizeButtonHint |
           Qt.Window

    onVisibleChanged: {
        if(this.visible){
            mainModel.mainWindowsLoaded();

            if(mainModel.internetAvalable)
                webView.url = model.mainPageUrl;
        }
    }

    Connections{
        target: model

        onOnInstallRequestShow:{
            licenseBlock.visible = true;
        }
        onOnInstallRequestFalse:{
            licenseBlock.visible = false;
        }
        onOnInternetActive:{
            webView.url = model.mainPageUrl;
        }
    }

    FontLoader{
        id: robotoRegularFont
        source: "qrc:///fonts/Roboto-Regular.ttf"
    }

    Image {
        id: backGroundImage
        width: 1000
        height: 700
        fillMode: Image.PreserveAspectCrop
        source: "qrc:///img/back.jpg"
        layer.enabled: true
        layer.effect: OpacityMask {
            maskSource: Item {
                width: mainWindow.width
                height: mainWindow.height
                Rectangle {
                    anchors.centerIn: parent
                    width: backGroundImage.width
                    height: backGroundImage.height
                    radius: 8
                }
            }
        }

    }

    // Веб виджет
    WebEngineView {
        id: webView
        anchors.bottomMargin: 160
        anchors.topMargin: 100
        anchors.fill: parent
        //url: "google.com"
        backgroundColor: "transparent"
        onLoadingChanged: {
            if(loadRequest.url !== webView.url && loadRequest.status === WebEngineLoadRequest.LoadStartedStatus){
                Qt.openUrlExternally(loadRequest.url);
                stop();
            }
        }

    }

    // Футер блок
    Rectangle{
        width: 1000
        id: footerRect
        height: 160;
        color: 'transparent'
        radius: 8
        anchors.bottom: parent.bottom
        anchors.bottomMargin: 0
        transformOrigin: Item.Bottom
        border.width: 0
        clip: true

        Rectangle {
            id: footerRectClipped
            x: 0
            y: -radius
            width: parent.width
            height: parent.height + radius
            radius: footerRect.radius
            border.width: 0
            color: "#cc260f49"
        }

        Item{
            width: parent.width
            height: parent.height
            visible: mainModel.isUserAuth

            FooterPlayBlock{
                id: footerPlayBlock
                anchors.rightMargin: 32
                anchors.leftMargin: 32
                anchors.bottomMargin: 28
                anchors.topMargin: 32
                anchors.fill: parent
                visible: ( (mainModel.installState === PlaceState.Play)
                        || (mainModel.installState === PlaceState.PlayProcess)
                        || (mainModel.installState === PlaceState.PlayReady)
                        || (mainModel.installState === PlaceState.HashChecking)) && !mainModel.launcherNeedUpdate
            }

            FooterUpdateBlock{
                id: footerUpdateBlock
                anchors.rightMargin: 32
                anchors.leftMargin: 32
                anchors.bottomMargin: 28
                anchors.topMargin: 32
                anchors.fill: parent
                visible: ( (mainModel.installState === PlaceState.Update)
                        || (mainModel.installState === PlaceState.UpdateProcess)
                        || (mainModel.installState === PlaceState.UpdateReady)) || mainModel.launcherNeedUpdate
            }


            FooterInstallBlock{
                id: footerInstallBlock
                anchors.rightMargin: 32
                anchors.leftMargin: 32
                anchors.bottomMargin: 28
                anchors.topMargin: 32
                anchors.fill: parent
                visible: ( (mainModel.installState === PlaceState.Install)
                        || (mainModel.installState === PlaceState.InstallProcess)
                        || (mainModel.installState === PlaceState.InstallReady)) && !mainModel.launcherNeedUpdate

            }
        }

        FooterLoginBlock{
            anchors.rightMargin: 32
            anchors.leftMargin: 32
            anchors.bottomMargin: 28
            anchors.topMargin: 32
            anchors.fill: parent
            visible: !mainModel.isUserAuth
        }

    }

    // Блок настроек
    Settings{
        id: settingsBlock
        width: parent.width
        height: parent.height
        visible: model.isSettings
    }

    // Заголовочный блок
    Rectangle{
        width: 1000
        id: headerRect
        height: 100;
        color: 'transparent'
        radius: 8
        border.width: 0
        clip: true

        MouseArea{
            property variant clickPos;
            anchors.fill: parent
            onPressed: {
                clickPos  = Qt.point(mouse.x,mouse.y);
            }

            onPositionChanged: {
                var delta = Qt.point(mouse.x-clickPos.x, mouse.y-clickPos.y);
                mainWindow.x += delta.x;
                mainWindow.y += delta.y;
            }
        }

        Rectangle {
            id: headerRectClipped
            x: 0
            y: 0
            width: parent.width
            height: parent.height + radius
            radius: headerRect.radius
            border.width: 0
            color: "#cc260f49"
        }

        Image{
            id: logo
            x: 32
            y: 31
            width: 229
            height: 38
            enabled: false
            antialiasing: true
            smooth: true
            fillMode: Image.Tile
            source: "qrc:///img/logo-white.svg"
        }

        HeaderUserBlock{
            anchors.top: parent.top
            anchors.topMargin: 35
            anchors.right: parent.right
            anchors.rightMargin: 200
            width: 150
            height: 30
        }

        HeaderWindowControl{
            anchors.top: parent.top
            anchors.topMargin: 35
            anchors.right: parent.right
            anchors.rightMargin: 23
            width: 96
            height: 30
        }

    }

    License{
        id: licenseBlock
        width: parent.width
        height: parent.height
        visible: false
    }

    PopapWarning{
        id: popapWarning
        width: parent.width
        height: parent.height
        visible: mainModel.showWarningMessage;
        onOnClose: {
            mainModel.setShowWarningMessage(false);
        }
    }

    function recursiveFindPosition(item, isX){
        var pos =isX ? 0 : 0;
        try{
            if(item.id !== mainWindow){
                pos =isX ? item.x : item.y;
                pos += recursiveFindPosition(item.parent,isX);
            }
        }catch(ex){

        }finally{
            return pos;
        }

    }

    function showInfo(item, text, diffX = 0, diffY = 0){

        infoHelper.visible = item.hovered;
        if(!item.hovered){
            return;
        }
        infoHelper.x = item.x + (item.width/2) - (infoHelper.width/2) + recursiveFindPosition(item.parent, true) + diffX;
        infoHelper.y = item.y - (infoHelper.height) + recursiveFindPosition(item.parent, false) + diffY;
        infoHelperText.text = text;
    }

    Rectangle{
        id: infoHelper
        height: 33
        width: infoHelperText.contentWidth + 50
        radius: 8
        color: "#322844"
        border.width: 1
        border.color: "#3d3253"
        visible: false

        Text {
            id: infoHelperText
            width: parent.width
            height: parent.height
            color: "#978fa5"
            text: "settings"
            verticalAlignment: Text.AlignVCenter
            horizontalAlignment: Text.AlignHCenter
            font.pixelSize: 13
            font.family: robotoRegularFont.name
        }
    }
}
