import QtQuick
import QtQuick.Controls
import Theme 1.0
import Server 1.0

Rectangle {
    id: item1
    height: 50
    //anchors.fill: parent
    color: "#273D53"

    property Server server
    signal click;
    signal removeClick;

    Label{
        id: title
        color: "#5c738a"
        text: server.name()
        anchors.left: parent.left
        anchors.top: parent.top
        font.pixelSize: 27
        anchors.leftMargin: 19
        anchors.topMargin: 9
    }

    MouseArea{
        cursorShape: Qt.PointingHandCursor
        anchors.fill: parent
        onClicked: {
            click();
        }
    }

    IconeButton{
        id: removeButton
        width: 45
        height: 45
        y: 1
        visible: true
        anchors.right: parent.right
        anchors.rightMargin: 5
        iconeUrl: "../static/img/RemoveButton.png"
        onOnClick: {
            removeClick();
        }
        tooltipString: "Удалить"
    }
}
