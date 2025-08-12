import QtQuick
import QtQuick.Controls
import Theme 1.0
import Server 1.0
import "../controls/."

Rectangle {
    color: "#00000000"

    property Server server;
    property int itemHeight: 80

    signal back();

    onVisibleChanged: {
        if(visible){
            itemHeight = 80
            spawnItems();
        }else
            listSettingsModel.clear();
    }

    function spawnItems(){
        listSettingsModel.clear();
        var settingsList = server.optionsQmlList();

        for(var i = 0 ; i < settingsList.length; i++){
            var item = settingsList[i];
            listSettingsModel.append({settElement: item});
        }
        baseScroll.contentHeight = settingsList.length * 45;
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
        text: server != null ? server.name() : ""
        width: 143
        color: Theme.textColor
        font.pointSize: 32
    }

    ScrollView{
        id: baseScroll
        anchors.left: parent.left
        anchors.right: parent.right
        anchors.top: parent.top
        anchors.bottom: parent.bottom
        anchors.bottomMargin: 30
        anchors.rightMargin: 0
        anchors.leftMargin: 0
        anchors.topMargin: 90
        smooth: true
        enabled: true
        focusPolicy: Qt.NoFocus
        clip: true
        ScrollBar.horizontal.policy: ScrollBar.AlwaysOff
        contentHeight: 700
        contentWidth: width

        ListView {
            id: listSettings
            width: parent.width
            height: parent.height
            spacing: -27

            model: ListModel {
                id: listSettingsModel
            }
            delegate: SettingsElementList{
                item: settElement
                width: baseScroll.width
            }
        }
    }
}


