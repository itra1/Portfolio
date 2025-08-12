import QtQuick 2.12
import QtQuick.Window 2.12
import QtQuick.Controls 2.4
import QtQuick.Controls.Material 2.3
import QtGraphicalEffects 1.0

Window {
    id: mainWindowLoader
    visible: true
    width: 400
    height: 150
    opacity: 1
    //title: qsTr("Title")
    color: "transparent"
    flags: Qt.FramelessWindowHint |
           Qt.WindowMinimizeButtonHint |
           Qt.Window

    Rectangle{
        id: rectangle
        width: parent.width
        height: parent.height
        color: "#2d2d2d"
        radius: 8
        border.color: "#3b3b3b"
    }

    Text {
        color: "#ffffff"
        text: qsTr("Loading")
        anchors.fill: parent
        verticalAlignment: Text.AlignVCenter
        horizontalAlignment: Text.AlignHCenter
        font.pixelSize: 18
    }

    Connections{
        target: mainModel

        onOnLoadComplete:{
            mainWindowLoader.close();
        }
    }

    FontLoader{
        id: robotoRegularFont
        source: "qrc:///fonts/Roboto-Regular.ttf"
    }
}
