import QtQuick
import QtQuick.Window
import QtQuick.Controls
import AppPhase 1.0
import "../controls/."

Window {
    id:baseWindow
    color: "#00000000"
    visible: true
    width: 1266
    height: 803
    flags: Qt.FramelessWindowHint | Qt.WindowMinimizeButtonHint | Qt.Window

    onWidthChanged: {
        resetSize();
    }


    onHeightChanged: {
        resetSize();
    }

    function resetSize(){
        baseWindow.width = 1266
        baseWindow.height = 803
    }

    Update{
        id: updateWindow
        visible: App.appState === AppPhase.UpdateCheck || App.appState === AppPhase.Update
        anchors.verticalCenter: parent.verticalCenter
        anchors.horizontalCenter: parent.horizontalCenter
    }

    Main{
        id: mainWindow
        visible: !updateWindow.visible
        anchors.fill: parent
        onMoveX: function(delta) {
            baseWindow.setX(baseWindow.x+delta);
        }
        onMoveY: function(delta) {
            baseWindow.setY(baseWindow.y+delta);
        }
        onMinimized: {
            baseWindow.showMinimized();
        }
    }
}
