import QtQuick
import QtQuick.Controls.Fusion
import QtQuick.Controls
import QtQuick.Dialogs
import QtQuick.Effects
import Theme 1.0

Item {
    id:baseItem
    height: 45
    width: 1000

    property string title
    property string value
    property alias inputMethodHit: inputField.inputMethodHints
    property alias echoMode: inputField.echoMode
    property alias tooltipString:thisTooltip.title
    property int fieldWidth: -1
    property bool isEnable: true

    signal valueChange(string value)

    Rectangle{
        id: titleRect
        height: parent.height
        width: Theme.settingsTitleWidth
        color: "#00000000"

        Text {
            color: isEnable ? Theme.textColor : Theme.disableColor
            text: title
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
            //popupX: baseRect.width
            //popupY: -baseRect.height
        }
    }

    Rectangle{
        height: parent.height
        width: fieldWidth < 0 ? parent.width-titleRect.width-50 : fieldWidth
        color: "#00000000"
        anchors.left: parent.left
        anchors.leftMargin: titleRect.width

        TextField {
            id: inputField
            width: parent.width
            height: parent.height
            enabled: baseItem.isEnable
            text: value
            color: baseItem.isEnable ? Theme.textColor : Theme.disableColor
            font.pointSize: 14
            placeholderTextColor: baseItem.isEnable ? Theme.textColor : Theme.disableColor
            anchors.horizontalCenterOffset: 0
            anchors.horizontalCenter: parent.horizontalCenter
            onTextEdited: {
                valueChange(inputField.text);
            }
            background: MultiEffect{
                Rectangle{
                    color: baseItem.isEnable ? Theme.fieldsFill : Theme.disableTransparentColor
                    anchors.fill: parent
                }
            }

        }
        Rectangle{
            height: 1
            color: baseItem.isEnable ? Theme.textColor : Theme.disableColor
            anchors.bottom: parent.bottom
            anchors.bottomMargin: 0
            width: parent.width
        }
    }

}
