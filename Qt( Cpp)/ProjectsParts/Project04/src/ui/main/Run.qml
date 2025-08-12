import QtQuick
import QtQuick.Controls

Item {
    width: 600
    height: 400


    Button {
        id: palyerButton
        x: 278
        y: 108
        width: 258
        height: 58
        text: qsTr("Запустить")
        anchors.horizontalCenterOffset: 0
        anchors.horizontalCenter: parent.horizontalCenter

        onClicked: {
            formManager.runWall();
        }
    }

    Button {
        id: clerCacheBrowser
        x: 278
        y: 219
        width: 258
        height: 34
        text: qsTr("Очистить кеш браузера")
        anchors.horizontalCenterOffset: 0
        anchors.horizontalCenter: parent.horizontalCenter

        onClicked: {
            formManager.clearBrowserCache();
        }
    }

    Button {
        id: clerAllCache
        x: 278
        y: 265
        width: 258
        height: 33
        text: qsTr("Очистить весь кеш")
        anchors.horizontalCenterOffset: 0
        anchors.horizontalCenter: parent.horizontalCenter

        onClicked: {
            formManager.clearFullCache();
        }
    }


}
