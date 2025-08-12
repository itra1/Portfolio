import QtQuick
import QtQuick.Controls
import QtQuick.Window
import Server 1.0
import Theme 1.0
import "../controls/."

Item {
    id: item1

    signal back;
    signal selectServer(Server srv);
    width: 1266
    height: 700

    FontLoader{
        id: golosRegularFont
        source: "../static/fonts/Golos_Text_Regular.ttf"
    }

    onVisibleChanged: {
        if(visible){
            createRect.visible = false;
            spawnItems();
        }else
            listModel.clear();
    }

    function spawnItems(){

        var serversList = ServersManager.getServersToQml();
        listModel.clear();

        for(var i = 0 ; i < serversList.length; i++){
            var server = serversList[i];
            listModel.append({serverIn: server});
        }
    }

    function removeServer(srv){
        ServersManager.removeServer(srv.id());
        spawnItems();
    }

    Rectangle{
        width:80
        height: 80
        color: "#00000000"
        x:10

        Image{
            anchors.fill: parent
            source: "../static/img/SettingsBack.png"
            anchors.rightMargin: 10
            anchors.leftMargin: 10
            anchors.bottomMargin: 10
            anchors.topMargin: 10
        }

        MouseArea{
            cursorShape: Qt.PointingHandCursor
            anchors.fill: parent
            onClicked: {
                back();
            }
        }
    }

    Text{
        id:titleLabel
        x: 100
        y: 15
        width: 143
        color: Theme.textColor
        font.pointSize: 32
        text: "Сервера"
    }

    Rectangle{
        id: serverList
        color: "#00000000"
        border.color: "#da2b455e"
        border.width: 1
        height: 500
        anchors.left: parent.left
        anchors.right: parent.right
        anchors.top: parent.top
        anchors.rightMargin: 30
        anchors.leftMargin: 30
        anchors.topMargin: 82

        StandartButton{
            id:addButton
            height: 50
            width: 250
            anchors.right: parent.right
            anchors.top: parent.top
            anchors.rightMargin: 10
            anchors.topMargin: 10
            title: "Добавить"
            onClick:{
                createRect.visible = true
            }
        }

        ScrollView{
            id: scrollView
            smooth: true
            enabled: true
            focusPolicy: Qt.NoFocus
            anchors.fill: parent
            anchors.rightMargin: 310
            anchors.leftMargin: 10
            anchors.bottomMargin: 10
            anchors.topMargin: 10
            ScrollBar.horizontal.policy: ScrollBar.AlwaysOff
            clip: true

            ListView {
                id: listView
                anchors.fill: parent
                spacing: 5
                boundsBehavior: Flickable.StopAtBounds

                model: ListModel {
                    id: listModel
                }
                delegate: ServerButton{
                    server: serverIn
                    width: scrollView.width != null ? scrollView.width - 10 : 0
                    height: 50
                    onClick: {
                        selectServer(serverIn);
                    }
                    onRemoveClick: {
                        removeServer(server);
                    }
                }
            }
        }

    }

    Rectangle{
        id: createRect
        color: "#d1313131"
        anchors.fill: parent
        opacity: 1

        onVisibleChanged: {
            if(visible){
                visibleAnimation.start();
            }
        }

        MouseArea{
            anchors.fill: parent
        }

        CreateServer{
            id:createServerPanel
            opacity: 1
            anchors.verticalCenter: parent.verticalCenter
            anchors.horizontalCenter: parent.horizontalCenter
            onOk:{
                spawnItems();
                invisibleAnimation.start();
            }
            onCancel: {
                invisibleAnimation.start();
            }
            onError: {
            }
        }

       ParallelAnimation{
           id: visibleAnimation
           PropertyAnimation{
               target: createServerPanel
               properties: "opacity"
               from: 0
               to: 1.0
           }

           PropertyAnimation{
               target: createRect
               properties: "opacity"
               from: 0
               to: 1.0
           }
       }

       ParallelAnimation{
           id: invisibleAnimation
           SequentialAnimation{
               PropertyAnimation{
                   target: createRect
                   properties: "opacity"
                   to: 0
               }
               PropertyAnimation{
                   target: createRect
                   properties: "visible"
                   //from: 0
                   to: false
               }
           }
           PropertyAnimation{
               target: createServerPanel
               properties: "opacity"
               to: 0
           }
       }
    }
}

/*##^##
Designer {
    D{i:0;formeditorZoom:0.5}
}
##^##*/
