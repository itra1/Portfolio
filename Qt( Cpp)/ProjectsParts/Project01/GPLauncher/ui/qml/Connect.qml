import QtQuick 2.0

Item {
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
            text: "Подключение..."
            anchors.fill: parent
            font.pixelSize: 14
            horizontalAlignment: Text.AlignHCenter
            verticalAlignment: Text.AlignVCenter
            font.family: montserratSemiBold.name
        }
    }
}

/*##^##
Designer {
    D{i:0;autoSize:true;height:480;width:640}
}
##^##*/
