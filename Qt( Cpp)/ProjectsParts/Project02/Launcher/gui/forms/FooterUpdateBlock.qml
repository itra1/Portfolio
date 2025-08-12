import QtQuick 2.11
import QtQuick.Controls 2.4
import PlaceState 1.0

Item {
    id: footerUpdateBlock

    function changeText(){
        if(footerUpdateBlock.visible){


            if(model.launcherNeedUpdate)
                installInfo.text = qsTr("Application is out of date. Update required.");
            else{
                if(model.updateSpaceNeed == "0"){
                    installInfo.text = qsTr("Version %1 is avalible.").arg(model.updateVersion);
                }else{
                    installInfo.text = qsTr("Version %1 is avalible. %2 required.").arg(model.updateVersion).arg(model.updateSpaceNeed);
                }
                console.log(model.updateSpaceNeed);
            }
        }
    }

    onVisibleChanged: {
        changeText();
    }

    Connections{
        target: model

        onOnStateChange:{
            changeText();
        }

        onOnInstallUpdate:{
            updateButtonRect.color = mainModel.isProcess ? "#9b9b9b" : "#00fff2"
        }
        onOnStateChangeAny:{
            updateButtonRect.color = mainModel.isProcess ? "#9b9b9b" : "#00fff2"
        }
    }

    FontLoader{
        id: robotoRegularFont
        source: "qrc:///fonts/Roboto-Regular.ttf"
    }

    Button{
        id: updateButton
        height: 64
        rightPadding: 30
        leftPadding: 30
        font.family: "Times New Roman"

        background: Rectangle{
            id: updateButtonRect
            width: parent.width
            height: parent.height
            color: updateButton.down ? "#c8f4b56a" : "#f4b56a"
        }

        contentItem: Text {
            id: playButtonText
            text: qsTr("UPDATE")
            font.capitalization: Font.AllUppercase
            font.weight: Font.ExtraLight
            font.pixelSize: 28
            verticalAlignment: Text.AlignVCenter
            horizontalAlignment: Text.AlignHCenter
            font.family: robotoRegularFont.name
        }
        onClicked: model.updateGame();
    }

    VersionChange{
        id: versionChange
        x: updateButton.width + 19
        width: 192
        height: 64
        visible: !mainModel.launcherNeedUpdate
    }

    ProgressLine{
        anchors.right: parent.right
        anchors.rightMargin: 0
        anchors.top: parent.top
        anchors.topMargin: 0
        width: parent.width - versionChange.width - updateButton.width - 19*2
        height: 97
        visible: mainModel.installState === PlaceState.UpdateProcess && !mainModel.launcherNeedUpdate
    }

    Text {
        id: installInfo
        y: 78
        width: parent.width
        height: 15
        color: "#73ffffff"
        text: ""
        anchors.left: parent.left
        anchors.leftMargin: 0
        anchors.bottom: parent.bottom
        anchors.bottomMargin: 0
        font.pixelSize: 14
        font.family: robotoRegularFont.name
    }
}






/*##^## Designer {
    D{i:0;autoSize:true;height:100;width:936}
}
 ##^##*/
