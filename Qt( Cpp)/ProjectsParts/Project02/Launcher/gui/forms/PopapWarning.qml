import QtQuick 2.11
import QtQuick.Controls 2.4

Item {
    id: root
    signal onClose();

    onVisibleChanged: {
        if(this.visible){
            console.log("onVisibleChanged");
            console.log(mainModel.getWarningMessage())
            installInfo.text = mainModel.getWarningMessage();
        }
    }

    Rectangle{
        id: backGround
        width: parent.width
        height: parent.height
        color: "#92201343"
    }

    Button{
        id: noProclickButton
        width: parent.width
        height: parent.height
        background: Image{
            width: parent.width
            height: parent.heigh
        }
    }

    Rectangle{
        id: lineBack
        y: parent.height/2 - height/2
        width: parent.width
        height: 208
        color: "#17131e"

        Text {
            id: installInfo
            width: 290
            height: 40
            color: "#ffffff"
            text: "";
            anchors.horizontalCenter: parent.horizontalCenter
            anchors.top: parent.top
            anchors.topMargin: parent.height*0.3
            wrapMode: Text.WordWrap
            verticalAlignment: Text.AlignVCenter
            horizontalAlignment: Text.AlignHCenter
            font.pixelSize: 16
            font.family: robotoRegularFont.name
        }

        Button {
            id: buttonRestoreSettings
            height: 32
            anchors.horizontalCenter: parent.horizontalCenter
            anchors.bottom: parent.bottom
            anchors.bottomMargin: 32
            leftPadding: 25
            rightPadding: 25
            spacing: 3
            clip: false

            background: Rectangle{
                width: parent.width
                height: parent.height
                color: buttonRestoreSettings.down ? "#509b9b9b" : "transparent"
                border.color: buttonRestoreSettings.down ? "#000000" : "#9b9b9b"
                border.width: 1
            }

            contentItem: Text {
                id: titleSettings
                text: qsTr("Close")
                font.pixelSize: 15
                verticalAlignment: Text.AlignVCenter
                color: buttonRestoreSettings.down ? "#000000" : "#9b9b9b"
            }
            onClicked: {
                console.log("onClicked");
                onClose();
            }
        }
    }
}




/*##^## Designer {
    D{i:0;autoSize:true;height:700;width:1000}
}
 ##^##*/
