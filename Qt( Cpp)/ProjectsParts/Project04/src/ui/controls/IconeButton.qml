import QtQuick

Rectangle {
    property bool isSelect: false
    property string tooltipString
    property bool isActive: true

    id: baseRect
    width: 34
    height: 34
    color: !isActive
           ? "00ffffff"
           : (mouseArea.containsMouse
            ? (mouseArea.pressed ? "#33000000" : "#33ffffff")
            : (isSelect ? "#ffffff" : "#00ffffff"))
    radius: 5

    opacity: isActive? 1 : 0.5

    property string iconeUrl;

    signal onClick;

    Image{
        id: icone
        source: iconeUrl
        anchors.fill: parent
        anchors.rightMargin: 5
        anchors.leftMargin: 5
        anchors.bottomMargin: 5
        anchors.topMargin: 5
    }

    MouseArea{
        id: mouseArea
        anchors.fill: parent
        cursorShape: isActive ? "PointingHandCursor" : "Default"
        hoverEnabled: isActive ? true : false
        onClicked:{
            onClick();
        }
        onEntered: {
            if(tooltipString.length > 0){
                thisTooltip.popupVisible = true;
            }
        }
        onExited: {
            if(tooltipString.length > 0){
                thisTooltip.popupVisible = false;
            }
        }
    }

    Tooltip{
        id: thisTooltip
        x: 0
        y: baseRect.height
        title: tooltipString
    }

}

/*##^##
Designer {
    D{i:0;formeditorZoom:6}
}
##^##*/
