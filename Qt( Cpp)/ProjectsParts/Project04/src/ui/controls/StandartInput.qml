import QtQuick
import QtQuick.Controls
import QtQuick.Effects
import QtQuick.Controls.Fusion

Rectangle{
    property alias aliasValue: inputField.text
    property alias inputMethodHit: inputField.inputMethodHints
    property alias validator: inputField.validator
    property alias echoMode: inputField.echoMode

    signal valueChange(string value)

    //height: parent.height
    //width: fieldWidth < 0 ? parent.width-titleRect.width-50 : 50
    color: "#00000000"
    anchors.left: parent.left
    anchors.leftMargin: titleRect.width
    TextField {
        id: inputField
        width: parent.width
        height: parent.height
        color: "#768bbb"
        font.pointSize: 14
        placeholderTextColor: "#768bbb"
        anchors.horizontalCenterOffset: 0
        anchors.horizontalCenter: parent.horizontalCenter
        onTextEdited: {
            valueChange(inputField.text);
        }

        background: MultiEffect{
            Rectangle{
                color: "#1aa2d1f2"
                // border.width: 0
                // height: parent.height
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
