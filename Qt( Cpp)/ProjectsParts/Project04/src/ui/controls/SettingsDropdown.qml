import QtQuick
import QtQuick.Controls
import Theme 1.0

Item {
    height: 50
    width: 1000
    required property string title
    required property string value;
    property alias selectList : renderStreamingType.model
    property string tooltipString
    signal valueChange(string value)

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

        MouseArea{
            id: mouseArea
            anchors.fill: parent
            //cursorShape: "PointingHandCursor"
            hoverEnabled: true
            //            onClicked:{
            //                onClick();
            //            }
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

    }
    Rectangle{
        id: rectangle
        height: parent.height
        width: parent.width-titleRect.width
        color: "#00000000"
        anchors.left: parent.left
        anchors.leftMargin: titleRect.width

        ComboBox{
            id: renderStreamingType
            height: parent.height
            width: 295
            font.pointSize: 14
            anchors.left: parent.left
            anchors.leftMargin: 0

            delegate: Rectangle {
                required property int index
                required property string modelData
                width: renderStreamingType.width
                height: renderStreamingType.height
                color: "#283a4a"

                Text {
                    x: 10
                    y: 10
                    width: renderStreamingType.width
                    text: modelData
                    color: "#768bbb"
                    font: renderStreamingType.font
                    elide: Text.ElideRight
                    verticalAlignment: Text.AlignVCenter
                    //styleColor: "#768bbb"
                }

                MouseArea{
                    anchors.fill: parent
                    cursorShape: "PointingHandCursor"

                    onEntered: {
                        parent.color = "#21303d";
                    }

                    onExited: {
                        parent.color = "#283a4a";
                    }

                    onClicked: {
                        renderStreamingType.currentIndex = index;
                        renderStreamingType.popup.visible = false;
                    }
                }

            }

            background: Rectangle{
                color: "#1aa2d1f2"
                border.width: 0
                height: 50
            }

            indicator: Canvas {
                id: canvas
                x: renderStreamingType.width - width - renderStreamingType.rightPadding
                y: renderStreamingType.topPadding + (renderStreamingType.availableHeight - height) / 2
                width: 12
                height: 8
                contextType: "2d"

                Connections {
                    target: renderStreamingType
                    function onPressedChanged() { canvas.requestPaint(); }
                }

                onPaint: {
                    context.reset();
                    context.moveTo(0, 0);
                    context.lineTo(width, 0);
                    context.lineTo(width / 2, height);
                    context.closePath();
                    context.fillStyle = renderStreamingType.pressed ? "#768bbb" : "#768bbb";
                    context.fill();
                }
            }

            popup: Popup {
                y: renderStreamingType.height - 1
                width: renderStreamingType.width
                implicitHeight: contentItem.implicitHeight
                padding: 1

                contentItem: ListView {
                    clip: true
                    implicitHeight: contentHeight
                    model: renderStreamingType.popup.visible ? renderStreamingType.delegateModel : null
                    currentIndex: renderStreamingType.highlightedIndex

                    ScrollIndicator.vertical: ScrollIndicator { }
                }

                background: Rectangle {
                    color: "#273749"
                    border.width: 0
                }
            }

            contentItem: Text {
                leftPadding: 10
                rightPadding: renderStreamingType.indicator.width + renderStreamingType.spacing

                text: renderStreamingType.displayText
                font: renderStreamingType.font
                color: renderStreamingType.pressed ? "#768bbb" : "#768bbb"
                verticalAlignment: Text.AlignVCenter
                elide: Text.ElideRight
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

    Tooltip{
        x: titleRect.x
        y: titleRect.height
        width: titleRect.width
        parentWidth:true
        id: thisTooltip
        //popupX: baseRect.width
        //popupY: -baseRect.height
        title: tooltipString
    }
}
