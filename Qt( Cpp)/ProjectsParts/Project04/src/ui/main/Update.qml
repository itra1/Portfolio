import QtQuick
import QtQuick.Window
import QtQuick.Controls

Item {
    id:infoWindow
    width: 359
    visible: true
    height: 174
    // color: "#00000000"
    // flags: Qt.FramelessWindowHint | Qt.WindowMinimizeButtonHint | Qt.Window

    Image {
        id: backGround
        anchors.fill: parent
        source: "../static/img/BackGroundMini.png"
        fillMode: Image.PreserveAspectFit
    }

    onVisibleChanged: {
        if(infoWindow.visible){
            UpdaterManager.qmlLoaded();
        }
    }

    Text {
        id: infoText
        color: "#ffffff"
        text: UpdaterManager.infoText
        anchors.fill: parent
        font.pixelSize: 12
        horizontalAlignment: Text.AlignHCenter
        verticalAlignment: Text.AlignVCenter
    }
}
