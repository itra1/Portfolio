import QtQuick
import QtQuick.Controls
import "../controls/."
import "../settings/."
import Server 1.0
import Theme 1.0

Item {
    id: item1
    width: 1266
    height: 803

    property int page: 0;

    onVisibleChanged: {
        if(visible)
            page = 0;
    }

    CommonSettingsPage{
        id: settingsBase
        anchors.fill: parent
        visible: page == 1
        onBack: {
            page = 0
        }
    }

    ServerSettingsPage{
        id: settingsServer
        anchors.fill: parent
        visible: page == 2
        onBack: {
            page = 4
        }
    }

    ServersList{
        id: listServers
        anchors.fill: parent
        visible: page == 4
        onBack: {
            page = 0
        }
        onSelectServer: function(srv){
            settingsServer.server = srv;
            page = 2;
        }
    }

    Rectangle {
        height: 500
        anchors.left: parent.left
        anchors.right: parent.right
        anchors.top: parent.top
        anchors.rightMargin: 50
        anchors.leftMargin: 50
        anchors.topMargin: 58
        color: "#00000000"
        visible: page == 0

        SettingsNavigationButton{
            id: baseSettings
            width: parent.width
            title: "Общие настройки"
            onClick: {
                settingsBase.titlePage = title
                page = 1;
            }
        }

        // SettingsNavigationButton{
        //     id: releaseServerSettings
        //     width: parent.width
        //     y: baseSettings.y + baseSettings.height+ 10
        //     title: "Сервер обновлений"
        //     onClick: {
        //         updateReleaseServer.titlePage = title
        //         page = 3;
        //     }
        // }

        SettingsNavigationButton{
            id: serverListSettings
            visible: Theme.settingsVisibleServers
            width: parent.width
            y: baseSettings.y + baseSettings.height+ 10
            title: "Сервера"
            onClick: {
                page = 4;
            }
        }
    }
}
