import QtQuick
import QtQuick.Controls

Rectangle {
    property alias title: title.text

    signal click;

    id: button
    radius:12
    width: 225
    height: 50
    color: "#1b4a8f"

    Label {
        id: title
        text: qsTr("Проверить")
        anchors.left: parent.left
        width: parent.width
        height: parent.height
        horizontalAlignment: Text.AlignHCenter
        verticalAlignment: Text.AlignVCenter
        anchors.leftMargin: 0
        font.family: golosRegularFont.name
        font.pointSize: 14
        color: "#ffffff"
    }

    MouseArea{
        anchors.fill: parent
        cursorShape: Qt.PointingHandCursor
        onClicked: {
            click();
        }
    }
}


