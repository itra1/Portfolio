import QtQuick 2.0
import QtQuick.Controls 2.4

Item {

    onVisibleChanged: {
        if(this.visible){
            userName.text = qsTr("<font color='#ffffff'><a href='%1' style='text-decoration:none; color:white;'>%2</a></font>").arg(mainModel.userLink).arg(mainModel.userName)
        }
    }

    id: element
    visible: mainModel.isUserAuth;

    Text {
        id: userName
        color: "#ffffff"
        text: "";
        anchors.top: parent.top
        anchors.topMargin: 4
        font.pixelSize: 15
        textFormat: Text.RichText
        anchors.right: parent.right
        anchors.rightMargin: 35
        linkColor: "#ffffff"
        verticalAlignment: Text.AlignVCenter
        horizontalAlignment: Text.AlignRight
        onLinkActivated: Qt.openUrlExternally(link);
    }

    Button {
        id: logout
        width: 32
        height: 30
        spacing: 3
        anchors.right: parent.right
        anchors.rightMargin: 0
        focusPolicy: Qt.NoFocus

        onHoveredChanged: {
            mainWindow.showInfo(this,qsTr("SingOut"));
        }

        contentItem: Image{
            anchors.rightMargin: 10
            anchors.leftMargin: 10
            anchors.bottomMargin: 9
            anchors.topMargin: 9
            anchors.fill: parent
            fillMode: Image.PreserveAspectFit
            sourceSize.height: 14
            sourceSize.width: 12
            source: "qrc:/img/logout.png"
        }

        background: Rectangle{
            width: logout.width
            height: logout.height
            color: "#ffffff"
            opacity: logout.down ? 0.5 : 0
        }

        onClicked: {
            mainModel.logOut();
        }
    }

}
