import QtQuick
import QtQuick.Controls
import QtQuick.Dialogs
import QtQuick.Effects
import QtQuick.Controls.Fusion
import Theme 1.0

Item {
    height: 130
    width: 1000

    required property string title
    property alias aliasUpValue: inputFieldUp.text
    property alias aliasDownValue: inputFieldDown.text
    property alias aliasLeftValue: inputFieldLeft.text
    property alias aliasrightValue: inputFieldRight.text
    signal valueUpChange(string value)
    signal valueDownChange(string value)
    signal valueLeftChange(string value)
    signal valueRightChange(string value)

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
        //width: fieldWidth < 0 ? parent.width-titleRect.width-50 : 50
        color: "#00000000"
        anchors.left: parent.left
        anchors.leftMargin: titleRect.width

        Image{
            id: image
            x: 0
            y: 0
            width: 300
            height: 129
            source: "../static/img/ScreenPreview.png"

            TextField {
                id: inputFieldUp
                width: 82
                height: 44
                color: "#768bbb"
                anchors.top: parent.top
                anchors.topMargin: 17
                anchors.horizontalCenter: parent.horizontalCenter
                font.pointSize: 14
                placeholderTextColor: "#768bbb"
                onTextEdited: {
                    valueUpChange(text);
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

            TextField {
                id: inputFieldDown
                y: 58
                width: 82
                height: 44
                color: "#768bbb"
                anchors.bottom: parent.bottom
                anchors.bottomMargin: 17
                font.pointSize: 14
                placeholderTextColor: "#768bbb"
                anchors.horizontalCenterOffset: 0
                anchors.horizontalCenter: parent.horizontalCenter
                onTextEdited: {
                    valueDownChange(text);
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

            TextField {
                id: inputFieldLeft
                y: 30
                width: 82
                height: 44
                color: "#768bbb"
                anchors.verticalCenter: parent.verticalCenter
                anchors.left: parent.left
                anchors.leftMargin: 17
                font.pointSize: 14
                placeholderTextColor: "#768bbb"
                onTextEdited: {
                    valueLeftChange(text);
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

            TextField {
                id: inputFieldRight
                y: 33
                width: 82
                height: 44
                color: "#768bbb"
                anchors.verticalCenter: parent.verticalCenter
                anchors.right: parent.right
                anchors.rightMargin: 17
                font.pointSize: 14
                placeholderTextColor: "#768bbb"
                onTextEdited: {
                    valueRightChange(text);
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
}

/*##^##
Designer {
    D{i:0;formeditorZoom:1.75}D{i:5}
}
##^##*/
