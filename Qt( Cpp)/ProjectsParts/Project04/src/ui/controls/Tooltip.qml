import QtQuick
import QtQuick.Controls
import QtQuick.Effects
import QtQuick.Controls.Fusion
import Theme 1.0

Item {
    id: parentItem
    property alias title: titleId.text
    property alias popupVisible: currentPopup.visible
    property bool parentWidth: false
    property alias popupX: currentPopup.x
    property alias popupY: currentPopup.y

    onVisibleChanged: {
        if(visible){
            if(parentWidth){
                titleId.width = parent.width;
                titleId.wrapMode = Text.WordWrap;
                currentPopup.width = parentItem.width
            }else{
                currentPopup.width = titleId.width+10
            }
            currentPopup.height = titleId.height+10
        }
    }

    FontLoader{
        id: golosRegularFont
        source: "../static/fonts/Golos_Text_Regular.ttf"
    }

    Popup{
        id: currentPopup
        height: 10
        width: 10
        padding: 1
        visible: false


        onVisibleChanged: {
            if(visible){
                if(!parentWidth)
                    currentPopup.width = titleId.width+10
                currentPopup.height = titleId.height+10
            }
        }

        Text {
            id: titleId
            x:5
            y:5
            color: Theme.textColor
            font.pointSize: 10
            font.family: golosRegularFont.name

            onVisibleChanged: {
                if(visible){
                    // console.log(currentPopup.width);
                    // console.log(currentPopup.height);
                    if(parentWidth){
                        currentPopup.width = parentItem.width;
                        currentPopup.width = Math.min(titleId.contentWidth+13,parentItem.width);
                        width = currentPopup.width;
                    }else
                        currentPopup.width = titleId.width+10
                }
            }
        }

        background: MultiEffect{
            Rectangle {
                color: "#273749"
                //color: Qt.transparent
                border.width: 0
                anchors.fill: parent
                radius: 5
            }
        }
    }
}

/*##^##
Designer {
    D{i:0;autoSize:true;height:480;width:640}
}
##^##*/
