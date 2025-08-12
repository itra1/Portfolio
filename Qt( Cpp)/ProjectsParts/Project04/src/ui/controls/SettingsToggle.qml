import QtQuick
import QtQuick.Controls
import Theme 1.0

Item {
    id:main
    height: 50
    width: 1000

    property alias title: titleRectName.text
    property alias isChecked: checkBox.isCheck
    property alias tooltipString:thisTooltip.title
    property bool isEnable: true

    signal valueChange(bool value)

    Rectangle{
        id: titleRect
        height: parent.height
        width: Theme.settingsTitleWidth
        color: "#00000000"

        Text {
            id: titleRectName
            color: isEnable ? Theme.textColor : Theme.disableColor
            anchors.fill: parent
            font.pixelSize: 18
            horizontalAlignment: Text.AlignRight
            verticalAlignment: Text.AlignVCenter
            anchors.rightMargin: 21
            font.family: golosRegularFont.name
            font.bold: false
        }

        MouseArea{
            id: mouseArea
            anchors.fill: parent
            hoverEnabled: isEnable
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
            x: titleRect.x
            y: titleRect.height
            width: titleRect.width
            parentWidth:true
        }

    }
    Rectangle{
        id: rectangle
        height: parent.height
        width: parent.width-titleRect.width
        color: "#00000000"
        anchors.left: parent.left
        anchors.leftMargin: titleRect.width

        CustomCheckBox{
            id: checkBox
            isEnable: main.isEnable
            anchors.left: parent.left
            anchors.top: parent.top
            anchors.leftMargin: 8
            anchors.topMargin: 13
            //isCheck: SettingsController.developMode;
            onOnChange: {
                valueChange(checkBox.isCheck);
            }
        }
    }
 }
