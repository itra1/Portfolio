import QtQuick 2.0

Item {
    id: item1
    width: 500
    height: 300
    FontLoader{
        id: montserratSemiBold
        source: "../fonts/Montserrat-SemiBold.otf"
    }

    Rectangle{
        width: 171
        height: 28
        color: "#8f000000"
        radius: 5
        anchors.bottom: parent.bottom
        anchors.bottomMargin: 73
        anchors.horizontalCenter: parent.horizontalCenter

        Text {
            color: "#ffffff"
            text: "Загрузка " + Math.round(Manager.downloadProgress < 0 ? 0 : Manager.downloadProgress * 100) + "%"
            anchors.fill: parent
            font.pixelSize: 14
            horizontalAlignment: Text.AlignHCenter
            verticalAlignment: Text.AlignVCenter
            font.family: montserratSemiBold.name
        }
    }

    Image {
        id: progress
        anchors.bottom: parent.bottom
        source: "qrc:///image/ProgressBarBack.png"
        //source: "../image/ProgressBarBack.png"
        anchors.bottomMargin: 25
        anchors.horizontalCenter: parent.horizontalCenter

//        Rectangle{
//            id: rectangle
//            color: "#16ffffff"
//            radius: 4.5
//            anchors.fill: parent
//            anchors.rightMargin: 6
//            anchors.leftMargin: 6
//            anchors.bottomMargin: 6
//            anchors.topMargin: 6



//            Image{
//                id:loadProgress
//                width: Manager.downloadProgress < 0 ? "0" : 395 * Manager.downloadProgress
//                anchors.left: parent.left
//                anchors.top: parent.top
//                anchors.bottom: parent.bottom
//                source: "qrc:///image/GoldGrad.png"
//                anchors.leftMargin: 0
//                anchors.bottomMargin: 0
//                fillMode: Image.Stretch

//            }

//        }

    }

    Rectangle{
        id: loadProgress
        y: 261
        width: Manager.downloadProgress < 0 ? "0" : 395 * Manager.downloadProgress
        //width: 395
        height: 7
        color: "red"
        radius: 10
        anchors.left: parent.left
        anchors.bottom: parent.bottom
        anchors.bottomMargin: 32
        anchors.leftMargin: 52
        gradient: Gradient {
            GradientStop { position: 0.0; color: "#c98f46" }
            GradientStop { position: 1.0; color: "#fbc47e" }
        }
    }
}

/*##^##
Designer {
    D{i:0;formeditorZoom:1.66}
}
##^##*/
