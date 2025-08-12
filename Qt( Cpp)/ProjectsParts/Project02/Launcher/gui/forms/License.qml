import QtQuick 2.11
import QtQuick.Controls 2.4

Item {
    id: licenseBlock
    width: 848
    height: 668

    Connections{
        target: model

        onOnLoadAgreement:{
            agreementText.text = model.getAgreement();
        }
    }

    onVisibleChanged: {
        if(licenseBlock.visible){
            agreementText.text = model.getAgreement();
        }
    }

    Rectangle{
        color: "#2d2d2d"
        radius: 8
        width: parent.width
        height: parent.height
    }

    FontLoader{
        id: robotoRegularFont
        source: "qrc:///fonts/Roboto-Regular.ttf"
    }

    FontLoader{
        id: robotoBoldFont
        source: "qrc:///fonts/Roboto-Bold.ttf"
    }

    // Заголовок
    Text{
        width: 238
        height: 37
        text: qsTr("License agreement")
        font.family: robotoBoldFont.name
        font.pixelSize: 28
        anchors.left: parent.left
        anchors.leftMargin: 36
        anchors.top: parent.top
        anchors.topMargin: 35
        color: "#ffffff"
    }

    // Кнопка закрытия
    Button {
        id: closeButton
        width: 32
        height: 32
        anchors.right: parent.right
        anchors.rightMargin: 24
        anchors.top: parent.top
        anchors.topMargin: 24

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
            model.agreementConfirm(false);
        }
    }


    Item{
        anchors.bottomMargin: 88
        anchors.leftMargin: 36
        anchors.rightMargin: 36
        anchors.topMargin: 96
        anchors.fill: parent

        Rectangle{
            width: parent.width
            height: parent.height
            color: "#212121"
        }

        ScrollView {
            id: scrollView
            x: 0
            y: 0
            clip: true
            contentWidth: parent.width - 28 - 28
            anchors.rightMargin: 28
            anchors.leftMargin: 28
            anchors.bottomMargin: 21
            anchors.topMargin: 21
            anchors.fill: parent

            Text{
                id:agreementText
                y: 0
                width: 100
                height: 1953
                color: "#ffffff"
                text: ""
                anchors.rightMargin: 3
                anchors.leftMargin: 3
                anchors.right: parent.right
                anchors.left: parent.left
                font.family: robotoRegularFont.name
                font.pixelSize: 14
                wrapMode: Text.WordWrap
                clip: false
                enabled: true
                fontSizeMode: Text.HorizontalFit
                renderType: Text.QtRendering
                textFormat: Text.AutoText
                linkColor: "#81dcd7"
                onLinkActivated: {
                    Qt.openUrlExternally(link);
                }
            }

        }

    }

    // Клавиша отмены
    Button{
        id: declineButton
        height: 32
        anchors.bottom: parent.bottom
        anchors.bottomMargin: 36
        anchors.left: parent.left
        anchors.leftMargin: 36

        contentItem: Text{
            height: 32
            color: "#9b9b9b"
            text: qsTr("Decline")
            verticalAlignment: Text.AlignVCenter
            horizontalAlignment: Text.AlignHCenter
            fontSizeMode: Text.VerticalFit
            font.family: robotoRegularFont.name
            font.pixelSize: 15
        }

        background: Rectangle{
            width: parent.width
            height: parent.height
            border.color: "#9b9b9b"
            color: "transparent"
            anchors.horizontalCenter: parent.horizontalCenter
            anchors.verticalCenter: parent.verticalCenter
        }

        onClicked: {
            model.agreementConfirm(false);
        }

    }

    Button{
        id: acceptButton
        anchors.right: parent.right
        anchors.rightMargin: 36
        anchors.bottom: parent.bottom
        anchors.bottomMargin: 36

        contentItem: Text{
            height: 32
            color: "#000000"
            text: qsTr("Accept")
            verticalAlignment: Text.AlignVCenter
            horizontalAlignment: Text.AlignHCenter
            anchors.horizontalCenter: parent.horizontalCenter
            anchors.verticalCenter: parent.verticalCenter
            font.family: robotoRegularFont.name
            font.pixelSize: 15
        }

        background: Rectangle{
            width: parent.width
            height: parent.height
            color: "#00fff2"
        }

        onClicked: {
            model.agreementConfirm(true);
        }

    }

}
