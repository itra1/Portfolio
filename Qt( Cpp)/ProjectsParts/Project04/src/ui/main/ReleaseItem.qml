import QtQuick
import QtQuick.Controls
import ReleaseState 1.0
import RunRelease 1.0
import "../controls/."
import AppUser 1.0
import Theme 1.0

Item {
    id: itemList
    width: 1159
    height: 100

    property RunRelease releaseRecord;

    property alias itemWidth: itemList.width
    property int itemHeight: 100
    property int infoHeight: 200
    //property AppUser user;

    onEnabledChanged: {
        //user = Authorization.getUser();
        playButtonIcone.opacity = Authorization.isRunWallAvailable() ? 1 : 0.5;
    }

    Connections{
        target: releaseRecord
        function onEmitDownloadError(errorVal){
            console.log("qml error " + errorVal);
        }
    }

    FontLoader{
        id: golosRegularFont
        source: "../static/fonts/Golos_Text_Regular.ttf"
    }

    Rectangle{
        color: "#273D53"
        width: parent.width
        height: parent.height
        radius: 12

        MouseArea{
            id: button
            width: parent.width
            height: parent.height
            onClicked: {
                if(itemList.height === itemHeight){
                    itemList.height = itemHeight +infoHeight;
                    infoData.visible = true;
                }else{
                    itemList.height = itemHeight;
                    infoData.visible = false;
                }
            }

            hoverEnabled: false
        }

        Text {
            x: 59
            y: 11
            width: 111
            height: 40
            color: "#5c738a"
            text: "Версия программы"
            anchors.top: parent.top
            font.pixelSize: 18
            font.family: golosRegularFont.name
            horizontalAlignment: Text.AlignRight
            lineHeight: 0.8
            wrapMode: Text.WordWrap
            anchors.topMargin: 30
            font.bold: false
        }

        Text {
            x: 196
            y: 11
            width: 123
            height: 40
            color: "#ffffff"
            //text: "0.5.26"
            text: releaseRecord.getVersion()
            anchors.top: parent.top
            font.pixelSize: 26
            verticalAlignment: Text.AlignVCenter
            fontSizeMode: Text.FixedSize
            font.family: golosRegularFont.name
            anchors.topMargin: 30
            font.bold: false
        }

        Rectangle{
            id: separate
            x: 341
            y: 12
            width: 1
            height: 75
            color: "#405870"
        }

        Rectangle{
            id: downloadButton
            x: 717
            y: 24
            width: 169
            height: 55
            color: "#00000000"
            visible: releaseRecord.state === ReleaseState.None

            Text {
                x: 0
                y: 11
                width: 93
                height: 41
                color: "#5c738a"
                text: "Загрузить программу"
                anchors.top: parent.top
                font.pixelSize: 18
                font.family: golosRegularFont.name
                horizontalAlignment: Text.AlignRight
                lineHeight: 0.8
                wrapMode: Text.WordWrap
                anchors.topMargin: 7
                font.bold: false
            }

            IconeButton{
                width: 56
                height: 56
                anchors.right: parent.right
                iconeUrl: "../static/img/DownloadButton.png"
                onOnClick: {
                    ReleaseManager.downloadItem(releaseRecord)
                }
            }

        }

        Rectangle{
            id: loadingButton
            x: 717
            y: 29
            width: 164
            height: 44
            visible: releaseRecord.state === ReleaseState.Loading
            color: "#00000000"

            Text {
                x: -38
                y: 11
                width: 131
                height: 40
                color: "#5c738a"
                text: "Идет загрузка"
                anchors.top: parent.top
                font.pixelSize: 18
                font.family: golosRegularFont.name
                horizontalAlignment: Text.AlignRight
                verticalAlignment: Text.AlignVCenter
                lineHeight: 0.8
                wrapMode: Text.WordWrap
                anchors.topMargin: 2
                font.bold: false
            }
            Rectangle {
                property double progressValue: 0.5

                id: dowloadProgress
                x: 118
                y: 12
                width: 296
                height: 18
                color: "#20354a"
                radius: 24
                border.color: "#38526c"
                border.width: 1

                Rectangle {
                    width: parent.width * releaseRecord.fullDownloadProgress
                    height: 18
                    color: "#2567c9"
                    radius: 24
                    border.width: 0


                    Text {
                        width: parent.width
                        height: parent.height
                        color: "#ffffff"
                        text: Math.round(releaseRecord.fullDownloadProgress*100) +"%"
                        anchors.top: parent.top
                        font.pixelSize: 13
                        horizontalAlignment: Text.AlignHCenter
                        verticalAlignment: Text.AlignVCenter
                        font.family: golosRegularFont.name
                        lineHeight: 0.8
                        wrapMode: Text.WordWrap
                        font.bold: false
                        visible: releaseRecord.fullDownloadProgress > 0.25
                    }
                }
            }
        }

        Rectangle{
            id: installButton
            x: 717
            y: 24
            width: 169
            height: 55
            visible: releaseRecord.state === ReleaseState.Downloaded
            color: "#00000000"

            Text {
                x: 0
                y: 11
                width: 93
                height: 40
                color: "#5c738a"
                text: "Установить на диск"
                anchors.top: parent.top
                font.pixelSize: 18
                font.family: golosRegularFont.name
                horizontalAlignment: Text.AlignRight
                lineHeight: 0.8
                wrapMode: Text.WordWrap
                anchors.topMargin: 7
                font.bold: false
            }

            IconeButton{
                width: 56
                height: 56
                anchors.right: parent.right
                iconeUrl: "../static/img/InstallButton.png"
                onOnClick: {
                    releaseRecord.install();
                }
            }

        }

        Rectangle{
            id: installLabel
            x: 717
            y: 29
            width: 164
            height: 44
            //onClicked: releaseRecord.run();
            color: "#00000000"
            visible: releaseRecord.state === ReleaseState.Unpack

            Text {
                x: 0
                y: 11
                width: 93
                height: 40
                color: "#5c738a"
                text: "Установить на диск"
                anchors.top: parent.top
                font.pixelSize: 18
                font.family: golosRegularFont.name
                horizontalAlignment: Text.AlignRight
                lineHeight: 0.8
                wrapMode: Text.WordWrap
                anchors.topMargin: 2
                font.bold: false
            }

            Text {
                x: 118
                y: 11
                width: 75
                height: 22
                color: "#ffffff"
                text: "Установка"
                anchors.top: parent.top
                anchors.topMargin: 10
                font.pixelSize: 18
                font.family: golosRegularFont.name
                font.bold: false

            }

        }

        Rectangle{
            id: playButton
            x: 717
            y: 24
            width: 169
            height: 55
            //onClicked: releaseRecord.run();
            visible: releaseRecord.state === ReleaseState.Installed
            color: "#00000000"

            Text {
                x: 0
                y: 11
                width: 93
                height: 40
                color: "#5c738a"
                text: "Запустить программу"
                anchors.top: parent.top
                font.pixelSize: 18
                font.family: golosRegularFont.name
                horizontalAlignment: Text.AlignRight
                lineHeight: 0.8
                wrapMode: Text.WordWrap
                anchors.topMargin: 7
                font.bold: false
            }

            IconeButton{
                id: playButtonIcone
                isActive: Session.isRunWallAvailable
                width: 56
                height: 56
                opacity: isActive ? 1 : 0.5
                anchors.right: parent.right
                iconeUrl: "../static/img/PlayButton.png"
                onOnClick: {
                    if(isActive)
                        releaseRecord.run();
                }
            }

            Text {
                x: 118
                y: 11
                width: 75
                height: 22
                color: "#ffffff"
                text: "Запущено"
                anchors.top: parent.top
                anchors.topMargin: 10
                font.pixelSize: 18
                font.family: golosRegularFont.name
                font.bold: false
                visible: releaseRecord.state === ReleaseState.Played || releaseRecord.state === ReleaseState.StartPlayed
            }
        }

        Rectangle{
            id: playerdButton
            x: 717
            y: 24
            width: 169
            height: 55
            //onClicked: releaseRecord.run();
            visible: releaseRecord.state === ReleaseState.Played || releaseRecord.state === ReleaseState.StartPlayed
            color: "#00000000"

            Text {
                x: 0
                y: 11
                width: 93
                height: 40
                color: "#5c738a"
                text: "Запустить программу"
                anchors.top: parent.top
                font.pixelSize: 18
                font.family: golosRegularFont.name
                horizontalAlignment: Text.AlignRight
                lineHeight: 0.8
                wrapMode: Text.WordWrap
                anchors.topMargin: 7
                font.bold: false
            }

            Text {
                x: 118
                y: 11
                width: 75
                height: 22
                color: "#ffffff"
                text: "Запущено"
                anchors.top: parent.top
                anchors.topMargin: 10
                font.pixelSize: 18
                font.family: golosRegularFont.name
                font.bold: false
            }
        }

        IconeButton{
            id: favoriteButton
            width: 45
            height: 45
            y: 28
            visible: releaseRecord.state !== ReleaseState.Loading
                     && releaseRecord.state !== ReleaseState.Unpack
            anchors.right: parent.right
            anchors.rightMargin: (releaseRecord.state === ReleaseState.Played || releaseRecord.state === ReleaseState.StartPlayed) ? 180 : 221
            iconeUrl: releaseRecord.isFavorite ? "../static/img/StarFill.png" : "../static/img/StarClear.png"
            onOnClick: {
                releaseRecord.favoriteToggle();
            }
            tooltipString: "В избранное"
        }

        IconeButton{
            id: openFolderLogButton
            width: 45
            height: 45
            y: 28
            visible: releaseRecord.state === ReleaseState.Installed
            anchors.right: parent.right
            anchors.rightMargin: 171
            iconeUrl: "../static/img/logClient.png"
            onOnClick: {
                Manager.openClientLog();
            }
            tooltipString: "Каталог с логом"
        }

        IconeButton{
            id: openFolderButton
            width: 45
            height: 45
            y: 28
            visible: releaseRecord.state === ReleaseState.Installed
            anchors.right: parent.right
            anchors.rightMargin: 121
            iconeUrl: "../static/img/FolderButton.png"
            onOnClick: {
                releaseRecord.openFolder();
            }
            tooltipString: "Открыть каталог с проектом"
        }

        IconeButton{
            id: shareButton
            width: 45
            height: 45
            y: 28
            visible: releaseRecord.state === ReleaseState.Installed
            anchors.right: parent.right
            anchors.rightMargin: 71
            iconeUrl: "../static/img/ShareButton.png"
            onOnClick: {
            }
            tooltipString: "Шаринг"
        }

        IconeButton{
            id: removeButton
            width: 45
            height: 45
            y: 28
            visible: releaseRecord.state === ReleaseState.Installed
            anchors.right: parent.right
            anchors.rightMargin: 21
            iconeUrl: "../static/img/RemoveButton.png"
            onOnClick: {
                releaseRecord.remove();
            }
            tooltipString: "Удалить"
        }

        Rectangle{
            id: infoData;
            visible: false
            anchors.top: parent.top
            anchors.topMargin: 100

            height: itemList.infoHeight
            width: parent.width
            color: Theme.fieldsFill

            ScrollView{
                leftPadding: 10
                rightPadding: 10
                topPadding: 10
                bottomPadding: 10
                width: parent.width - leftPadding - rightPadding
                height: parent.height - topPadding - bottomPadding
                ScrollBar.horizontal.policy: ScrollBar.AlwaysOff
                clip: true
                background: Rectangle {
                    color: "#00000000"
                }

                Text {
                    id: descriptionItem
                    text: releaseRecord.getDescription()
                    font.pixelSize: 14
                    width: infoData.width -30
                    clip: true
                    wrapMode: Text.WordWrap
                    color: "#ffffff"

                }
            }
        }

    }
    Rectangle{
        height: 5
    }
}

/*##^##
Designer {
    D{i:0;formeditorZoom:1.5}
}
##^##*/
