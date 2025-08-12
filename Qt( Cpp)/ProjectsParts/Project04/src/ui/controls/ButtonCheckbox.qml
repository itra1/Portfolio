import QtQuick

Rectangle{
    property int state: 0

    signal click;

    id: button
    radius:12
    width: 225
    height: 50
    color: "#1b4a8f"

    onVisibleChanged: {
        if(visible){
            state = 0
        }
    }

    function setState(value){
        flag.state = value;
    }

    Text {
        id: title
        text: qsTr("Проверить")
        anchors.left: parent.left
        width: parent.width
        height: parent.height
        horizontalAlignment: Text.AlignHCenter
        verticalAlignment: Text.AlignVCenter
        anchors.leftMargin: 0
        font.family: golosRegularFont.name
        font.pointSize: 14
        color: "#ffffff"
    }

    Image{
        id: flag
        width: 30
        height: 30
        anchors.verticalCenter: parent.verticalCenter
        anchors.right: parent.right
        source: state == 0 ? "../static/img/CheckNone.png"
                           : state == 1 ? "../static/img/CheckError.png"
                                        : "../static/img/CheckOk.png"
        anchors.rightMargin: 10
    }

    MouseArea{
        anchors.fill: parent
        cursorShape: Qt.PointingHandCursor
        onClicked: {
            click();
        }
    }

}

