import QtQuick 2.10
import QtQuick.Controls 2.4
import PlaceState 1.0

Item {

    onVisibleChanged: {
        if(this.visible){
            //infoText.text = qsTr("Size: %1 of %2 remainig. Speed: %3").arg(0).arg(0).arg(0);
            progressLine.width = (progressLine.parent.width-40) * 0;
            progressText.text = model.isCheckingProgress
                              ? qsTr("Checking %1%").arg(Math.round(0))
                              : qsTr("Downloading %1%").arg(Math.round(0))
            progressText.horizontalAlignment = Text.AlignLeft
            progressText.x = -11;
            progressText.anchors.rightMargin = -9;
            progressText.color = "#ffffff"
        }
    }

    Connections{
        target: model

        onOnInstallUpdate:{
            //infoText.text = qsTr("Size: %1 of %2 remainig. Speed: %3").arg(model.readySize).arg(model.fullSize).arg(model.speedLoad);
            progressLine.width = (progressLine.parent.width-40) * model.progress;
            //progressText.visible = model.progress > 0.05;
            progressText.text = model.isCheckingProgress
                              ? qsTr("Checking %1%").arg(Math.round(model.progress*100))
                              : qsTr("Downloading %1%").arg(Math.round(model.progress*100))
            progressText.horizontalAlignment = model.progress < 0.5 ? Text.AlignLeft : Text.AlignRight;
            progressText.x = model.progress < 0.5 ? 11 : -11;
            progressText.anchors.rightMargin = model.progress < 0.5 ? -9 : 9;
            progressText.color = model.progress < 0.5 ? "#ffffff" : "#000000"
        }
    }


    FontLoader{
        id: robotoBoldFont
        source: "qrc:///fonts/Roboto-Bold.ttf"
    }


    Rectangle{
        id: rectangle
        width: parent.width
        height: 64
        color: "#20000000"

        Rectangle{
            id: progressLine
            width: 100
            height: parent.height-40
            color: "#00fff2"
            anchors.top: parent.top
            anchors.topMargin: 20
            anchors.left: parent.left
            anchors.leftMargin: 20

            Text{
                id: progressText
                x: 11
                width: 1
                text: "0%"
                anchors.bottom: parent.bottom
                anchors.bottomMargin: 2
                anchors.top: parent.top
                anchors.topMargin: 2
                verticalAlignment: Text.AlignVCenter
                horizontalAlignment: Text.AlignRight
                anchors.right: parent.right
                anchors.rightMargin: 9
                font.pixelSize: 14
                font.family: robotoBoldFont.name
            }
        }
    }

    Button{
        id: button
        height: 18
        spacing: 4
        font.pointSize: 7
        font.family: "Verdana"
        anchors.bottom: parent.bottom
        anchors.bottomMargin: 0
        anchors.right: parent.right
        anchors.rightMargin: 0
        display: AbstractButton.TextBesideIcon

        background: Rectangle{
            width: parent.width
            height: parent.height
            color: "transparent"
        }

        indicator:Image{
            width:8; height:10;
            anchors.left: parent.left
            anchors.leftMargin: 0
            anchors.top: parent.top
            anchors.topMargin: 4
            horizontalAlignment:Image.AlignRight
            source: mainModel.isPauseTorrent ? "qrc:///img/playIcone.png" : "qrc:///img/pauseIcone.png"

        }

        contentItem: Text {
            id: playButtonText
            color: "#75ffffff"
            text: mainModel.isPauseTorrent ? qsTr("PLAY") : qsTr("PAUSE")
            anchors.right: parent.right
            anchors.rightMargin: 0
            font.pixelSize: 11
            verticalAlignment: Text.AlignVCenter
            horizontalAlignment: Text.AlignRight
            font.family: robotoRegularFont.name

        }
        onClicked: {
            model.pauseDownload();
        }
    }
}























/*##^## Designer {
    D{i:0;autoSize:true;height:97;width:514}
}
 ##^##*/
