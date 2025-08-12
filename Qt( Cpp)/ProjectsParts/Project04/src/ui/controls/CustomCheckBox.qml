import QtQuick
import Theme 1.0
//import "./../."

Rectangle {

    property bool isCheck: false
    property real margin: 5
    property bool isEnable:true

    signal onChange;

    id: main
    width: 30
    height: 30
    color: isEnable ? Theme.fieldsFill : Theme.disableTransparentColor
    border.color: isEnable ? Theme.inputsBorder : Theme.disableColor
    border.width: 1

    Rectangle{
        color: isEnable ? Theme.inputsBorder : Theme.disableColor
        anchors.fill: parent
        anchors.rightMargin: margin
        anchors.leftMargin: margin
        anchors.bottomMargin: margin
        anchors.topMargin: margin
        visible: isCheck
    }

    MouseArea{
        anchors.fill: parent
        cursorShape: isEnable ? "PointingHandCursor" : "ArrowCursor"
        hoverEnabled: isEnable
        enabled: isEnable
        onClicked: {
            isCheck = !isCheck
            onChange();
        }
    }
}

/*##^##
Designer {
    D{i:0;formeditorZoom:10}
}
##^##*/
