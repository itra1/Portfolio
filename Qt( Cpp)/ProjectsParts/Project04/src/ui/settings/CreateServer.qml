import QtQuick
import Theme 1.0
import Server 1.0
import "../controls/."

Rectangle {
    id: rectangle
    height: 200
    width: 400
    color: "#1a2a39"
    radius: 13

    signal ok;
    signal cancel;
    signal error;

    onVisibleChanged: {
        if(visible){
            serverName.aliasValue = "";
        }
    }

    FontLoader{
        id: golosRegularFont
        source: "../static/fonts/Golos_Text_Regular.ttf"
    }

    Text{
        y: 11
        color: "#ffffff"
        text:"Название"
        font.pixelSize: 32
        anchors.horizontalCenter: parent.horizontalCenter
    }

    SequentialAnimation{
        id: errorAnimation
        NumberAnimation{ target: rectangle; property: "anchors.horizontalCenterOffset"; to: 10; duration: 50}
        NumberAnimation{ target: rectangle; property: "anchors.horizontalCenterOffset"; to: -10; duration: 100}
        NumberAnimation{ target: rectangle; property: "anchors.horizontalCenterOffset"; to: 10; duration: 100}
        NumberAnimation{ target: rectangle; property: "anchors.horizontalCenterOffset"; to: -10; duration: 100}
        NumberAnimation{ target: rectangle; property: "anchors.horizontalCenterOffset"; to: 0; duration: 50}
    }

    StandartInput{
        id: serverName
        x: 56
        height: 46
        anchors.right: parent.right
        anchors.top: parent.top
        anchors.topMargin: 64
        anchors.leftMargin: 30
        anchors.rightMargin: 30
        aliasValue: ""
        inputMethodHit: Qt.ImhNone
        onValueChange: {
            //server.name = value;
        }
    }


    StandartButton{
        id:okButton
        width: 180
        height: 50
        anchors.left: parent.left
        anchors.bottom: parent.bottom
        anchors.leftMargin: 15
        anchors.bottomMargin: 15
        title: "Создать"
        onClick:{
            if(serverName.aliasValue.length <= 2){
                errorAnimation.start();
                error();
                return;
            }

            ServersManager.createServer(serverName.aliasValue)
            ok();
        }
    }


    StandartButton{
        id:cancelButton
        x: 205
        y: 135
        height: 50
        width: 180
        anchors.right: parent.right
        anchors.bottom: parent.bottom
        anchors.bottomMargin: 15
        anchors.rightMargin: 15
        title: "Отмена"
        onClick:{
            cancel();
        }
    }
}
