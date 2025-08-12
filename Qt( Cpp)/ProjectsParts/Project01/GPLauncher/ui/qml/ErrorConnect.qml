import QtQuick 2.0
import QtQuick.Controls 2.12

Item {
    id: errorWindow
    width: 500
    height: 300
    FontLoader{
        id: montserratSemiBold
        source: "../fonts/Montserrat-SemiBold.otf"
    }
    onVisibleChanged: {
        if(errorWindow.visible){
            errorText.text = Manager.getError();
            errorRect.width = errorText.contentWidth + errorText.anchors.rightMargin + errorText.anchors.leftMargin;
            errorRect.height = errorText.contentHeight + errorText.anchors.topMargin + errorText.anchors.bottomMargin;
        }
    }


//        Image {
//            id: name
//            anchors.fill: parent
//            source: "../image/template1.png"
//            anchors.leftMargin: -1
//            anchors.topMargin: -1
//        }

    Rectangle{
        id:errorRect
        width: 381
        height: 44
        color: "#8f000000"
        radius: 5
        anchors.bottom: parent.bottom
        anchors.bottomMargin: 73
        anchors.horizontalCenter: parent.horizontalCenter

        Text {
            id: errorText
            color: "#ffffff"
            text: ""
            anchors.fill: parent
            font.pixelSize: 14
            horizontalAlignment: Text.AlignHCenter
            verticalAlignment: Text.AlignVCenter
            wrapMode: Text.WordWrap
            anchors.rightMargin: 15
            anchors.leftMargin: 15
            anchors.bottomMargin: 5
            anchors.topMargin: 5
            font.family: montserratSemiBold.name
        }
    }

    Rectangle{
        id: closeButton
        y: 243
        width: 211
        height: 34
        anchors.bottom: parent.bottom
        anchors.bottomMargin: 22
        anchors.horizontalCenter: parent.horizontalCenter
        color: "#00000000"

        Image{
            anchors.fill: parent
            source: "../image/GoldButton.png"
            anchors.rightMargin: -3
            anchors.leftMargin: -3
            anchors.bottomMargin: -5
        }
        Text {
            color: "#4b371e"
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
                Manager.errorRepeatClickButton();
            }
        }

    }

//    Button{
//        visible: false
//        width: 211
//        height: 34
//        anchors.bottom: parent.bottom
//        anchors.bottomMargin: 22
//        anchors.horizontalCenter: parent.horizontalCenter
//        background: Image{
//            source: "../image/GoldButton.png"
//            anchors.rightMargin: -3
//            anchors.leftMargin: -3
//            anchors.bottomMargin: -5
//        }
//        onClicked: {
//            Manager.errorRepeatClickButton();
//        }

//        Text{
//            color: "#4b371e"
//            text: "Повторить"
//            anchors.fill: parent
//            horizontalAlignment: Text.AlignHCenter
//            verticalAlignment: Text.AlignVCenter
//            anchors.rightMargin: 0
//            anchors.leftMargin: 0
//            anchors.bottomMargin: 0
//            anchors.topMargin: 0
//            font.family: montserratSemiBold.name
//        }
//    }

}


