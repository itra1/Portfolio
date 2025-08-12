import QtQuick
import QtQuick.Controls
import QtQuick.Dialogs
import QtQuick.Effects
import QtQuick.Controls.Fusion
import Theme 1.0

Item {
    id:main
    height: 46
    width: 1000

    required property string title
    property alias aliasValue: inputField.text
    property alias inputMethodHit: inputField.inputMethodHints
    property alias validator: inputField.validator
    property alias echoMode: inputField.echoMode
    property alias fileDialogFilter: fileDialog.nameFilters
    property alias tooltipString:thisTooltip.title
    property bool useFileDialog: false
    property int fieldWidth: -1
    property string fileDialogtitle: "Открыть файл"
    signal valueChange(string value)
    property bool isEnable: true

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
    }

    Rectangle{
        height: parent.height
        width: fieldWidth < 0
               ? parent.width-titleRect.width-parent.height - 15
               : 50
        color: "#00000000"
        anchors.left: parent.left
        anchors.leftMargin: titleRect.width

        TextField {
            id: inputField
            width: parent.width
            height: parent.height
            color: isEnable ? Theme.textColor : Theme.disableColor
            font.pointSize: 14
            placeholderTextColor: isEnable ? Theme.textColor : Theme.disableColor
            anchors.horizontalCenterOffset: 0
            anchors.horizontalCenter: parent.horizontalCenter
            onTextEdited: {
                valueChange(inputField.text);
            }
            background: MultiEffect{
                Rectangle{
                    color: isEnable ? Theme.fieldsFill : Theme.disableTransparentColor
                    anchors.fill: parent
                }
            }


        }
        Rectangle{
            height: 1
            color: isEnable ? Theme.textColor : Theme.disableColor
            anchors.bottom: parent.bottom
            anchors.bottomMargin: 0
            width: parent.width
        }

        MouseArea{
            id: mouseArea
            anchors.fill: parent
            hoverEnabled: true
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

    Rectangle {
        id: selectButton
        width: parent.height
        height: parent.height
        activeFocusOnTab: true
        radius:5
        border.width: 0
        anchors.right: parent.right
        anchors.rightMargin: 10
        color: isEnable ? Theme.textColor : Theme.disableColor
        visible: useFileDialog

        Text {
            id: tt
            text: qsTr("...")
            width: parent.width
            height: parent.height
            horizontalAlignment: Text.AlignHCenter
            verticalAlignment: Text.AlignVCenter
            font.family: golosRegularFont.name
            font.pointSize: 14
            color: "#000000"
        }

        MouseArea{
            id: enterButtonMA
            anchors.fill: parent
            enabled: isEnable
            cursorShape: isEnable ? "PointingHandCursor" : "ArrowCursor"
            hoverEnabled: isEnable
            onEntered: {
                selectButton.color =  "#a9b9de"
            }

            onExited: {
                selectButton.color = isEnable ? Theme.textColor : Theme.disableColor
            }

            onClicked: {
                fileDialog.title = fileDialogtitle;
                fileDialog.open();
            }
        }

    }

    FileDialog {
        id: fileDialog
        onAccepted: {
            var url = fileDialog.fileUrl;
            console.log(url);
            valueChange(url);
        }
    }
}
