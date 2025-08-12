import QtQuick 2.11
import QtQuick.Controls 2.4
import PlaceState 1.0

Item {
    id: footerInstallBlock

    onVisibleChanged: {
        if(footerInstallBlock.visible){
            installInfo.text = qsTr("Game is not installed. %1 required.").arg(model.updateSpaceNeed);
            installButtonRect.color = mainModel.isProcess ? "#9b9b9b" : "#00fff2"
        }
    }

    Connections{
        target: mainModel

        onOnInstallUpdate:{
            updateText();

        }
        onOnStateChange:{
            if(footerInstallBlock.visible){
                updateText();
            }
        }
        onOnStateChangeAny:{
            updateText();
        }
    }

    function updateText(){
        if(mainModel.installState === PlaceState.InstallProcess)
            installInfo.text = qsTr("Size: %1 of %2 remainig. Speed: %3").arg(model.readySize).arg(model.fullSize).arg(model.speedLoad);
        else{
            installInfo.text = mainModel.avalableInstallVersion ?
                    qsTr("Game is not installed. %1 required.").arg(model.updateSpaceNeed)
                      : qsTr("No avalable version");
        }
        installButtonRect.color = mainModel.isProcess ? "#9b9b9b" : "#00fff2"
    }

    FontLoader{
        id: robotoRegularFont
        source: "qrc:///fonts/Roboto-Regular.ttf"
    }

    Button{
        id: installButton
        height: 64
        rightPadding: 30
        leftPadding: 30

        background: Rectangle{
            id: installButtonRect
            width: parent.width
            height: parent.height
            color: installButton.down ? "#c800fff2" : "#00fff2"
        }

        contentItem: Text {
            id: playButtonText
            x: 8
            y: 6
            text: qsTr("INSTALL")
            fontSizeMode: Text.VerticalFit
            renderType: Text.NativeRendering
            font.family: robotoRegularFont.name
            font.capitalization: Font.AllUppercase
            font.pixelSize: 28
            verticalAlignment: Text.AlignVCenter
            horizontalAlignment: Text.AlignHCenter
        }

        onClicked: {
            model.installButton();
        }

    }

    VersionChange{
        id: versionChange
        x: installButton.width + 19
        width: 192
        height: 64
    }

    ProgressLine{
        anchors.right: parent.right
        anchors.rightMargin: 0
        anchors.top: parent.top
        anchors.topMargin: 0
        width: parent.width - versionChange.width - installButton.width - 19*2
        height: 97
        visible: mainModel.installState === PlaceState.InstallProcess
    }

    Text {
        id: installInfo
        width: parent.width
        height: 15
        color: "#73ffffff"
        text: ""
        textFormat: Text.RichText
        anchors.left: parent.left
        anchors.leftMargin: 0
        anchors.bottom: parent.bottom
        anchors.bottomMargin: 0
        font.pixelSize: 14
        font.family: robotoRegularFont.name
//        onLinkActivated:
//                        {
//                            console.log(link)
//                            Qt.openUrlExternally(link)
//                        }
        onLinkActivated: console.log(link + " link activated")
    }
}































/*##^## Designer {
    D{i:0;autoSize:true;height:100;width:936}D{i:5;anchors_x:8;anchors_y:6}
}
 ##^##*/
