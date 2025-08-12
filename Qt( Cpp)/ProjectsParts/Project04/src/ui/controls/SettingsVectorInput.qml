import QtQuick
import QtQuick.Controls
import QtQuick.Dialogs
import QtQuick.Effects
import QtQuick.Controls.Fusion
import Theme 1.0

Item {
    height: 46
    width: 1000

    required property string title
    property alias aliasXValue: inputXField.text
    property alias inputXMethodHit: inputXField.inputMethodHints
    property alias aliasYValue: inputYField.text
    property alias inputYMethodHit: inputYField.inputMethodHints
    signal valueXChange(string value)
    signal valueYChange(string value)

    Rectangle{
        id: titleRect
        height: parent.height
        width: Theme.settingsTitleWidth
        color: "#00000000"

        Text {
            color: Theme.textColor
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
        width: 100
        color: "#00000000"
        anchors.left: parent.left
        anchors.leftMargin: titleRect.width + 20

        Text {
            color: Theme.textColor
            text: "X:"
            anchors.left: parent.left
            anchors.top: parent.top
            anchors.bottom: parent.bottom
            font.pixelSize: 18
            horizontalAlignment: Text.AlignRight
            verticalAlignment: Text.AlignVCenter
            anchors.leftMargin: -19
            font.family: golosRegularFont.name
            font.bold: false
        }

        TextField {
            id: inputXField
            width: parent.width
            height: parent.height
            color: "#768bbb"
            font.pointSize: 14
            placeholderTextColor: "#768bbb"
            anchors.horizontalCenterOffset: 0
            anchors.horizontalCenter: parent.horizontalCenter
            onTextEdited: {
                valueXChange(inputXField.text);
            }

            background: MultiEffect{
                Rectangle{
                    color: "#1aa2d1f2"
                    anchors.fill: parent
                }
            }

            Rectangle{
                height: 1
                color: "#768bbb"
                anchors.bottom: parent.bottom
                anchors.bottomMargin: 0
                width: parent.width
            }
        }
    }

    Rectangle{
        height: parent.height
        width: 100
        color: "#00000000"
        anchors.left: parent.left
        anchors.leftMargin: titleRect.width + 150

        Text {
            color: Theme.textColor
            text: "Y:"
            anchors.left: parent.left
            anchors.top: parent.top
            anchors.bottom: parent.bottom
            font.pixelSize: 18
            horizontalAlignment: Text.AlignRight
            verticalAlignment: Text.AlignVCenter
            anchors.leftMargin: -19
            font.family: golosRegularFont.name
            font.bold: false
        }

        TextField {
            id: inputYField
            width: parent.width
            height: parent.height
            color: "#768bbb"
            font.pointSize: 14
            placeholderTextColor: "#768bbb"
            anchors.horizontalCenterOffset: 0
            anchors.horizontalCenter: parent.horizontalCenter
            onTextEdited: {
                valueYChange(inputYField.text);
            }

            background: MultiEffect{
                Rectangle{
                    color: "#1aa2d1f2"
                    anchors.fill: parent
                }
            }

            Rectangle{
                height: 1
                color: "#768bbb"
                anchors.bottom: parent.bottom
                anchors.bottomMargin: 0
                width: parent.width
            }
        }
    }

}
