import QtQuick 2.12
import QtGraphicalEffects 1.0

Item {
    id: item1
    width: 500
    height: 300

    FontLoader{
        id: montserratSemiBold
        source: "../fonts/Montserrat-SemiBold.otf"
    }

    Rectangle{
        y: 176
        width: 259
        height: 44
        color: "#8f000000"
        radius: 5
        anchors.bottom: parent.bottom
        anchors.bottomMargin: 80
        anchors.horizontalCenter: parent.horizontalCenter

        Text {
            color: "#ffffff"
            text: "Network error:\nInternet connection lost"
            anchors.fill: parent
            font.pixelSize: 14
            horizontalAlignment: Text.AlignHCenter
            verticalAlignment: Text.AlignVCenter
            font.family: montserratSemiBold.name
        }
    }


    Rectangle{
        id: closeButton
        y: 243
        width: 120
        height: 34
        anchors.bottom: parent.bottom
        anchors.horizontalCenterOffset: -65
        anchors.bottomMargin: 24
        anchors.horizontalCenter: parent.horizontalCenter
        color: "#00000000"

        Image{
            anchors.fill: parent
            source: "../image/CloseButton.png"
            anchors.rightMargin: -3
            anchors.leftMargin: -3
            anchors.bottomMargin: -5
        }
        Text {
            color: "#ffffff"
            text: "Close"
            anchors.fill: parent
            font.pixelSize: 14
            horizontalAlignment: Text.AlignHCenter
            verticalAlignment: Text.AlignVCenter
            anchors.bottomMargin: 4
            font.family: montserratSemiBold.name
        }
        MouseArea{
            anchors.fill: parent
            cursorShape: Qt.PointingHandCursor

            onClicked: {
                App.quit();
            }
        }

    }

    Rectangle{
        id: tryAgaine
        y: 243
        width: 120
        height: 34
        anchors.bottom: parent.bottom
        anchors.horizontalCenterOffset: 65
        anchors.bottomMargin: 24
        anchors.horizontalCenter: parent.horizontalCenter
        color: "#00000000"

        Image{
            anchors.fill: parent
            source: "../image/TryAgaineButton.png"
            anchors.rightMargin: -3
            anchors.leftMargin: -3
            anchors.bottomMargin: -5
        }
        Text {
            id: tryAgaineText
            color: "#2D1700"
            text: "Try again"
            anchors.fill: parent
            font.pixelSize: 12
            horizontalAlignment: Text.AlignHCenter
            verticalAlignment: Text.AlignVCenter
            font.bold: false
            anchors.bottomMargin: 4
            font.family: montserratSemiBold.name
        }



        MouseArea{
            anchors.fill: parent
            cursorShape: Qt.PointingHandCursor

            onClicked: {
                App.tryConnectAgaine();
            }
        }

    }

}


