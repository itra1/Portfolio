import QtQuick 2.0
import QtQuick.Controls 2.4

Row {
    id: headerWindowControl
    width: 96
    height: 30
    transformOrigin: Item.TopRight


    Button {
        id: settingButton
        width: 32
        height: 30
        focusPolicy: Qt.NoFocus
        enabled: true
        checkable: false

        onHoveredChanged: {
            mainWindow.showInfo(this,qsTr("Settings"));
        }

        contentItem: Image{
            anchors.right: parent.right
            anchors.rightMargin: 10
            anchors.left: parent.left
            anchors.leftMargin: 10
            anchors.top: parent.top
            anchors.topMargin: 9
            fillMode: Image.PreserveAspectFit
            sourceSize.height: 12
            sourceSize.width: 12
            anchors.bottom: parent.bottom
            anchors.bottomMargin: 9
            source: "qrc:///img/iocn-settings.svg"
        }

        background: Rectangle{
            width: settingButton.width
            height: settingButton.height
            color: "#ffffff"
            opacity: settingButton.down ? 0.5 : 0
        }

        onClicked: {
            model.settingsButton();
        }

    }

    Button {
        id: minimizationButton
        width: 32
        height: 30

        onHoveredChanged: {
            mainWindow.showInfo(this,qsTr("Minimize"));
        }

        contentItem: Image{
            anchors.rightMargin: 10
            anchors.leftMargin: 10
            anchors.bottomMargin: 9
            anchors.topMargin: 19
            anchors.fill: parent
            transformOrigin: Item.Center
            fillMode: Image.PreserveAspectFit
            sourceSize.height: 2
            sourceSize.width: 12
            source: "qrc:///img/icon-minimize.svg"
        }

        background: Rectangle{
            width: minimizationButton.width
            height: minimizationButton.height
            color: "#ffffff"
            opacity: minimizationButton.down ? 0.5 : 0
        }

        onClicked: {
            mainWindow.showMinimized();
        }
    }

    Button {
        id: closeButton
        width: 32
        height: 30

        onHoveredChanged: {
            mainWindow.showInfo(this,qsTr("Close"),-20);

        }

        contentItem: Image{
            anchors.rightMargin: 10
            anchors.leftMargin: 10
            anchors.bottomMargin: 9
            anchors.topMargin: 9
            anchors.fill: parent
            fillMode: Image.PreserveAspectFit
            sourceSize.height: 12
            sourceSize.width: 12

            source: "qrc:///img/icon-cross.svg"
        }

        background: Rectangle{
            width: closeButton.width
            height: closeButton.height
            color: "#ffffff"
            opacity: closeButton.down ? 0.5 : 0
        }

        onClicked: {
            model.closeApplication();
        }
    }


}
