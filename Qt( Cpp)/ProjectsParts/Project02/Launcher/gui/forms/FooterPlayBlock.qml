import QtQuick 2.11
import QtQuick.Controls 2.4
import PlaceState 1.0

Item {
    id: footerPlayBlock

    onVisibleChanged: {
        if(footerPlayBlock.visible){
            showInfoText();
            playButtonRect.color = mainModel.isProcess ? "#9b9b9b" : "#00fff2"
        }
    }

    Connections{
        target: model

        onOnInstallUpdate:{
            if(mainModel.installState === PlaceState.HashChecking)
                installInfo.text = qsTr("Size: %1 of %2 remainig. Speed: %3").arg(model.readySize).arg(model.fullSize).arg(model.speedLoad);
            else
                showInfoText();

            playButtonRect.color = mainModel.isProcess ? "#9b9b9b" : "#00fff2"
        }

        onOnStateChange:{
            if(!footerPlayBlock.visible)
                return;
            if(mainModel.installState === PlaceState.HashChecking)
                installInfo.text = qsTr("Size: %1 of %2 remainig. Speed: %3").arg(model.readySize).arg(model.fullSize).arg(model.speedLoad);
            else
                showInfoText();
        }

        onOnStateChangeAny:{
            playButtonRect.color = mainModel.isProcess ? "#9b9b9b" : "#00fff2"
        }


    }

    function showInfoText(){
        installInfo.text = qsTr("Game is up to date. Version %1. View the <font color='#ffffff'><a href='%2'>patch notes</a></font>.").arg(model.versionInstallReady).arg(model.noteUrl);
    }

    FontLoader{
        id: robotoRegularFont
        source: "qrc:///fonts/Roboto-Regular.ttf"
    }

    Button{
        id: playButton
        width: 192;
        height: 64
        rightPadding: 30
        leftPadding: 30
        font.capitalization: Font.AllUppercase

        background: Rectangle{
            id: playButtonRect
            width: parent.width
            height: parent.height
            color: model.isPlay ? ("#d1d1d1")
                                : (playButton.down ? "#c800fff2" : "#00fff2")
        }

        contentItem: Text {
            id: playButtonText
            text: qsTr("PLAY")
            font.capitalization: Font.AllUppercase
            font.pixelSize: 28
            verticalAlignment: Text.AlignVCenter
            horizontalAlignment: Text.AlignHCenter
            font.family: robotoRegularFont.name
        }
        onClicked: {
            model.playButton();
        }
    }

    VersionChange{
        id: versionChange
        x: playButton.width + 19
        width: 192
        height: 64
    }

    ProgressLine{
        anchors.right: parent.right
        anchors.rightMargin: 0
        anchors.top: parent.top
        anchors.topMargin: 0
        width: parent.width - versionChange.width - playButton.width - 19*2
        height: 97
        visible: mainModel.installState === PlaceState.HashChecking
    }

    Text {
        id: installInfo
        y: 78
        height: 15
        color: "#73ffffff"
        linkColor: "#ffffffff"
        text: ""
        anchors.right: parent.right
        anchors.rightMargin: 0
        anchors.left: parent.left
        anchors.leftMargin: 0
        anchors.bottom: parent.bottom
        anchors.bottomMargin: 0
        font.pixelSize: 14
        font.family: robotoRegularFont.name
        onLinkActivated: Qt.openUrlExternally(link);
    }
}
/*##^## Designer {
    D{i:0;autoSize:true;height:100;width:936}
}
 ##^##*/
