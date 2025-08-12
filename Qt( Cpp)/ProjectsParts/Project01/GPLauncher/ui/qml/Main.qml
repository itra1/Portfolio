import QtQuick 2.15
import QtQuick.Window 2.15
import QtQuick.Controls 2.12
import QtGraphicalEffects 1.0
import AppPhase 1.0

Window {
    id: window
    width: 500
    height: 300
    visible: true
    flags: Qt.FramelessWindowHint |
           Qt.WindowMinimizeButtonHint |
           Qt.Window

    FontLoader{
        id: montserratSemiBold
        source: "../fonts/Montserrat-SemiBold.otf"
    }

    AnimatedImage {
        id: background
        anchors.fill: parent
        source: "qrc:///image/BackAnim.gif"
        anchors.leftMargin: -1
        anchors.topMargin: -1
    }
//    Image {
//        id: name
//        anchors.fill: parent
//        source: "qrc:///image/template1.png"
//        anchors.leftMargin: -1
//        anchors.topMargin: -1
//    }


    Connect{
        width: parent.width
        height: parent.height
        visible: Manager.appPhase === AppPhase.Connect && !App.isConnectLost
    }
    Download{
        width: parent.width
        height: parent.height
        visible: Manager.appPhase === AppPhase.Download && !App.isConnectLost
    }
    ErrorConnect{
        width: parent.width
        height: parent.height
        visible: Manager.appPhase === AppPhase.Error && !App.isConnectLost
    }

    ConnectionLost{
        width: parent.width
        height: parent.height
        visible: App.isConnectLost
    }

    Text{
        id: logText
        color: "#ff0000"
        text: App.logText;
        anchors.fill: parent
        //visible: false
    }

    Text{
        id: version
        color: "#7c7c7c"
        text: App.appVersion;
        anchors.fill: parent
        horizontalAlignment: Text.AlignRight
        verticalAlignment: Text.AlignBottom
        styleColor: "#99000000"
        bottomPadding: 2
        rightPadding: 2
        anchors.leftMargin: 393
        anchors.topMargin: 285
        font.pointSize: 10
    }

}








