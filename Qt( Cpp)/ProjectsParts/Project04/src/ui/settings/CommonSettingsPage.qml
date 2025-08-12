import QtQuick
import QtQuick.Controls
import BaseSettings 1.0
import Theme 1.0
import "./../controls/."
import "."
import SettingsItem 1.0

Rectangle {
    id: rectangle

    property BaseSettings settings;
    property alias titlePage: titleLabel.text

    property int itemHeight: 10
    property bool isExtend: false

    color: "#00000000"

    signal back();

    onVisibleChanged: {
        if(visible){
            itemHeight = 80
            settings = SettingsController.getBaseSettingsQObject();
            spawnItems();
        }else
            listSettingsModel.clear();
    }

    function spawnItems(){
        listSettingsModel.clear();
        var settingsList = settings.optionsQmlList();

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
        id: titleLabel
        x: 100
        y: 15
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
        //        ScrollBar.vertical.policy: ScrollBar.AlwaysOn
        contentHeight: 700
        contentWidth: width

        ListView {
            id: listSettings
            //width: baseScroll.width != null ? baseScroll.width : 0
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

/*##^##
Designer {
    D{i:0;autoSize:true;height:480;width:640}
}
##^##*/
