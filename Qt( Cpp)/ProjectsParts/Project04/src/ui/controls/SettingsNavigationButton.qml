import QtQuick
import Theme 1.0

Item {
    id: itemId
    width: 1000
    height: 50

    property alias title: title.text

    signal click()

    Rectangle{
        id: rectangle
        color: "#16222f"
        anchors.fill: parent

        Text{
            id: title
            color: Theme.textColor
            text: "Заголовок"
            anchors.verticalCenter: parent.verticalCenter
            anchors.left: parent.left
            font.pixelSize: 26
            lineHeightMode: Text.ProportionalHeight
            anchors.verticalCenterOffset: -3
            font.hintingPreference: Font.PreferDefaultHinting
            anchors.leftMargin: 41
            font.family: "Verdana"
        }
    }

    MouseArea{
        anchors.fill: parent
        cursorShape: Qt.PointingHandCursor

        onClicked:{
            click()
        }
    }
}
