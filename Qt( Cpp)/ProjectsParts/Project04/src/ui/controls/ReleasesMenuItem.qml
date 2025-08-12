import QtQuick
import Theme 1.0

Rectangle{

    property string colorNoSelect: "#5c738a"
    property string colorHover: "#84a2c0"

    property alias title: mainLabel.text
    property bool isSelected: false
    property bool isHover: false

    signal clickButton;

    id: buttonItem
    width: mainLabel.paintedWidth
    height: mainLabel.paintedHeight
    color: "#00000000"

    onVisibleChanged: {
        buttonItem.width = mainLabel.contentWidth;
    }

    Text {
        id: mainLabel
        font.pixelSize: 33
        font.family: golosRegularFont.name
        color: isSelected
                        ? Theme.mainTextColor
                        :  isHover
                            ? colorHover
                            : colorNoSelect;
    }

    MouseArea{
        anchors.fill: parent
        cursorShape: "PointingHandCursor"
        hoverEnabled: true
        onClicked:{
            clickButton();
        }
        onEntered: {
            isHover = true;
        }
        onExited: {
            isHover = false;
        }
    }
}
