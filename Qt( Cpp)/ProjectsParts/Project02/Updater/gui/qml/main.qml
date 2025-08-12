import QtQuick 2.12
import QtQuick.Window 2.12
import QtQuick.Controls 2.4
import InstalState 1.0

Window {
    id: mainWindow
    visible: true
    width: 456
    height: 200
    opacity: 1
    title: qsTr("Title")
    color: "transparent"
    flags: Qt.FramelessWindowHint |
           Qt.WindowMinimizeButtonHint |
           Qt.Window

    Rectangle{
        id: rectangle
        width: mainWindow.width
        height: mainWindow.height
        color: "#2d2d2d"
        radius: 8
        border.color: "#3b3b3b"
    }

    FontLoader {
        id: robotoRegularFonts
        source: "qrc:///fonts/Roboto-Regular.ttf"
    }

    FontLoader {
        id: robotoBoldFonts
        source: "qrc:///fonts/Roboto-Bold.ttf"
    }

    FontLoader {
        id: robotoRegularThin
        source: "qrc:///fonts/Roboto-Thin.ttf"
    }


    Item{
        id: startBlock
        width: mainWindow.width
        height: mainWindow.height
        visible: model.state == InstalState.Init

        Text {
            id: startBlockTitle
            width: 142
            height: 20
            color: "#ffffff"
            text: qsTr("Checking for updates")
            anchors.horizontalCenterOffset: 0
            anchors.top: parent.top
            anchors.topMargin: 80
            anchors.horizontalCenter: parent.horizontalCenter
            font.family: robotoRegularFonts.name
            font.capitalization: Font.AllLowercase
            font.pixelSize: 15
            verticalAlignment: Text.AlignVCenter
            horizontalAlignment: Text.AlignHCenter
        }

        Image {
            id: image
            x: 178
            y: 117
            width: 48
            height: 8
            anchors.bottom: parent.bottom
            anchors.bottomMargin: 72
            anchors.horizontalCenter: parent.horizontalCenter
            fillMode: Image.PreserveAspectFit
            source: "qrc:///img/group.svg"
        }
    }

    Item{
        id: installBlock
        width: mainWindow.width
        height: mainWindow.height
        visible: model.state == InstalState.Process

        Text {
            id: installBlockTitle
            width: 142
            height: 20
            color: "#ffffff"
            text: qsTr("Installing update")
            font.bold: false
            anchors.horizontalCenterOffset: 0
            anchors.top: parent.top
            anchors.topMargin: 36
            anchors.horizontalCenter: parent.horizontalCenter
            font.family: robotoBoldFonts.name
            font.pixelSize: 15
            verticalAlignment: Text.AlignVCenter
            horizontalAlignment: Text.AlignHCenter
        }

        Text {
            id: percentText
            width: 93
            height: 66
            color: "#00fff2"
            text: ((Math.round((10000*model.progress)))/100).toString() +" %"
            anchors.horizontalCenterOffset: 0
            anchors.top: parent.top
            anchors.topMargin: 68
            anchors.horizontalCenter: parent.horizontalCenter
            font.family: robotoRegularThin.name
            font.capitalization: Font.AllLowercase
            font.pixelSize: 50
            verticalAlignment: Text.AlignVCenter
            horizontalAlignment: Text.AlignHCenter
        }

        Rectangle{


            id: lineLoaderBack
            width: 320
            height: 12
            color: "#242424"
            anchors.horizontalCenter: parent.horizontalCenter
            anchors.bottom: parent.bottom
            anchors.bottomMargin: 40

            Rectangle{
                width: 312 * model.progress
                height: 4
                color: "#00fff2"
                anchors.left: parent.left
                anchors.top: parent.top
                transformOrigin: Item.Left
                anchors.leftMargin: 4
                anchors.topMargin: 4
            }

        }

    }

}










