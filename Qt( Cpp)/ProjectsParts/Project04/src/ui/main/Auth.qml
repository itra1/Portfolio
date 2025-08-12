import QtQuick
import QtQuick.Controls
import QtQuick.Controls.Fusion
import QtQuick.Effects
import "../controls/."
import Theme 1.0
//import CustomControls

Item {
    id: authform
    width: 1266
    height: 803
    layer.samplerName: "s"

    property string attnKeys: ""
    property bool waitIsDevelop: true

    Component.onCompleted: {
        init();
    }

    onVisibleChanged: {
        if(visible)
        init();
    }

    Connections{
        target: Authorization;
        function onIsAuthStart(status){
        }
    }

    function init(){
        spawnServersList();

        login.text = Authorization.userName();
        password.text = Authorization.password();
        remember.isCheck = Authorization.remember();
    }

    function spawnServersList(){
        var serversList = ServersManager.getServersToQml();
        listServers.clear();
        var serverIndex = 0;
        var serverId = Authorization.serverId();

        var ii = -1;

        for(var i = 0 ; i < serversList.length; i++){
            var serverItem = serversList[i];

            if(serverItem.isValid()){
                ii++;
                listServers.append({modelData: serverItem.name()});
                if(serverItem.id() === serverId)
                    serverIndex = ii
            }
        }

        server.currentIndex = serverIndex;
    }

    function getServerId(index){

        var serversList = ServersManager.getServersToQml();
        var ii = -1;

        for(var i = 0 ; i < serversList.length; i++){
            var server = serversList[i];

            if(server.isValid()){
                ii++;
                if(ii === index)
                    return server.id();
            }
        }
    }

    function addKey(val){
        if(!waitIsDevelop) return;
        var keyDevelop = "011211";
        attnKeys += val;
        if(attnKeys.length < keyDevelop.length) return;

        if(attnKeys.substring(attnKeys.length-7) == keyDevelop){
            waitIsDevelop = false;
        }
    }

    Item{
        id: item1
        y: 201
        width: 293
        height: 400
        anchors.horizontalCenter: parent.horizontalCenter

        Text {
            id: authTitle
            y: errorMessage.visible ? 0 : 50
            width: 250
            height: 29
            color: "#ffffff"
            text: qsTr("Авторизация")
            font.pixelSize: 24
            font.family: golosRegularFont.name
            horizontalAlignment: Text.AlignHCenter
            verticalAlignment: Text.AlignVCenter
            anchors.horizontalCenter: parent.horizontalCenter
        }

        Rectangle {
            id: errorMessage
            y: 50
            width: 295
            height: 50
            color: "#00000000"
            anchors.horizontalCenter: parent.horizontalCenter
            visible: Manager.isloginError

            //Locked error
            Rectangle{
                id:errorMessageLocked
                anchors.fill: parent
                border.color: "#f67777"
                color: "#30e29d9d"
                border.width: 1
                visible: Manager.loginErrorText == "Locked"

                Image {
                    id: lockedIcone
                    source: Qt.resolvedUrl("/forms/LauncherQml/src/ui/static/img/LockedIcone.png")
                    x:5
                    y:5
                    width: 40
                    height: 40
                }

                Text {
                    id: errorMessageText1
                    width: parent.width
                    height: parent.height
                    color: "#ff2424"
                    text: "Пользователь заблокирован"
                    elide: Text.ElideNone
                    anchors.verticalCenter: parent.verticalCenter
                    font.pixelSize: 15
                    horizontalAlignment: Text.AlignHCenter
                    verticalAlignment: Text.AlignVCenter
                    wrapMode: Text.WordWrap
                    anchors.horizontalCenter: parent.horizontalCenter
                    fontSizeMode: Text.FixedSize
                    renderType: Text.QtRendering
                    textFormat: Text.RichText
                    clip: true
                    minimumPointSize: 12
                    minimumPixelSize: 10
                    leftPadding: 45
                }
            }
            //Bad Request error
            Rectangle{
                id:errorMessageBadRequest
                anchors.fill: parent
                border.color: "#f67777"
                color: "#30e29d9d"
                border.width: 1
                visible: Manager.loginErrorText == "Bad Request"

                Image {
                    id: errorIcone
                    source: Qt.resolvedUrl("/forms/LauncherQml/src/ui/static/img/ErrorIcone.png")
                    x:5
                    y:5
                    width: 40
                    height: 40
                }

                Text {
                    id: errorMessageText2
                    width: parent.width
                    height: parent.height
                    color: "#ff2424"
                    text: "Ошибка обработки"
                    elide: Text.ElideNone
                    anchors.verticalCenter: parent.verticalCenter
                    font.pixelSize: 15
                    horizontalAlignment: Text.AlignHCenter
                    verticalAlignment: Text.AlignVCenter
                    wrapMode: Text.WordWrap
                    anchors.horizontalCenter: parent.horizontalCenter
                    fontSizeMode: Text.FixedSize
                    renderType: Text.QtRendering
                    textFormat: Text.RichText
                    clip: true
                    minimumPointSize: 12
                    minimumPixelSize: 10
                    leftPadding: 45
                }
            }
            //Other error
            Rectangle{
                id:errorMessageOther
                anchors.fill: parent
                border.color: "#f67777"
                color: "#30e29d9d"
                border.width: 1
                visible: Manager.loginErrorText != "Locked" && Manager.loginErrorText != "Bad Request"

                Text {
                    id: errorMessageText
                    width: parent.width
                    height: parent.height
                    color: "#ff2424"
                    text: Manager.loginErrorText
                    elide: Text.ElideNone
                    anchors.verticalCenter: parent.verticalCenter
                    font.pixelSize: 15
                    horizontalAlignment: Text.AlignHCenter
                    verticalAlignment: Text.AlignVCenter
                    wrapMode: Text.WordWrap
                    anchors.horizontalCenter: parent.horizontalCenter
                    fontSizeMode: Text.FixedSize
                    renderType: Text.QtRendering
                    textFormat: Text.RichText
                    clip: true
                    minimumPointSize: 12
                    minimumPixelSize: 10
                }
            }
        }

        Rectangle{
            id: authFormRect
            y: errorMessage.visible ? errorMessage.y + errorMessage.height + 20 : authTitle.y + 50
            width: 295

            TextField {
                id: login
                y: 0
                width: parent.width
                height: 50
                color: "#768bbb"
                font.pointSize: 14
                placeholderTextColor: "#768bbb"
                anchors.horizontalCenterOffset: 0
                anchors.horizontalCenter: parent.horizontalCenter
                placeholderText: qsTr("Логин")

                //background:
                background: MultiEffect{
                    Rectangle{
                        color: "#1aa2d1f2"
                        anchors.fill: parent
                    }
                }

                Rectangle{
                    height: 1
                    color: "#768bbb"
                    anchors.bottom: parent.bottom
                    anchors.bottomMargin: 0
                    width: parent.width
                }
            }

            TextField {
                id: password
                y: 60
                width: parent.width
                height: 50
                visible: true
                color: "#768bbb"
                echoMode: TextInput.Password
                anchors.horizontalCenterOffset: 0
                anchors.horizontalCenter: parent.horizontalCenter
                placeholderText: qsTr("Пароль")
                placeholderTextColor: "#768bbb"
                font.pointSize: 14

                background: MultiEffect{
                    Rectangle{
                        color: "#1aa2d1f2"
                        anchors.fill: parent
                        //height: 50
                    }
                }

                Rectangle{
                    height: 1
                    color: "#768bbb"
                    anchors.bottom: parent.bottom
                    anchors.bottomMargin: 0
                    width: parent.width
                }
            }

            Rectangle{
                y: 112

                CustomCheckBox{
                    id: remember
                    anchors.left: parent.left
                    anchors.top: parent.top
                    anchors.leftMargin: 8
                    anchors.topMargin: 13
                    Text{
                        x: 42
                        y: 3
                        text: qsTr("Запомнить пользователя")
                        color: "#768bbb"
                        font.pointSize: 14
                    }
                }

            }

            ComboBox{
                id: server
                y: 171
                height: 50
                width: parent.width
                font.pointSize: 14
                anchors.horizontalCenter: parent.horizontalCenter
                model: ListModel {
                    id: listServers
                }

                delegate: Rectangle {
                    required property int index
                    required property string modelData
                    width: server.width
                    height: server.height
                    color: "#283a4a"

                    Text {
                        x: 10
                        y: 10
                        width: server.width
                        text: modelData
                        color: "#768bbb"
                        font: server.font
                        elide: Text.ElideRight
                        verticalAlignment: Text.AlignVCenter
                        //styleColor: "#768bbb"
                    }

                    MouseArea{
                        anchors.fill: parent
                        cursorShape: "PointingHandCursor"

                        onEntered: {
                            parent.color = "#21303d";
                        }

                        onExited: {
                            parent.color = "#283a4a";
                        }

                        onClicked: {
                            server.currentIndex = index;
                            server.popup.visible = false;
                        }
                    }

                }

                background: MultiEffect{
                    Rectangle{
                        color: "#1aa2d1f2"
                        anchors.fill: parent
                    }
                }

                indicator: Canvas {
                    id: canvas
                    x: server.width - width - server.rightPadding
                    y: server.topPadding + (server.availableHeight - height) / 2
                    width: 12
                    height: 8
                    contextType: "2d"

                    Connections {
                        target: server
                        function onPressedChanged() { canvas.requestPaint(); }
                    }

                    onPaint: {
                        context.reset();
                        context.moveTo(0, 0);
                        context.lineTo(width, 0);
                        context.lineTo(width / 2, height);
                        context.closePath();
                        context.fillStyle = server.pressed ? "#768bbb" : "#768bbb";
                        context.fill();
                    }
                }

                popup: Popup {
                    y: server.height - 1
                    width: server.width
                    implicitHeight: contentItem.implicitHeight
                    padding: 1

                    contentItem: ListView {
                        clip: true
                        implicitHeight: contentHeight
                        model: server.popup.visible ? server.delegateModel : null
                        currentIndex: server.highlightedIndex

                        ScrollIndicator.vertical: ScrollIndicator { }
                    }

                    background: MultiEffect{
                        Rectangle {
                            color: "#273749"
                            anchors.fill: parent
                        }
                    }

                }

                contentItem: Text {
                    leftPadding: 10
                    rightPadding: server.indicator.width + server.spacing

                    text: server.displayText
                    font: server.font
                    color: server.pressed ? "#768bbb" : "#768bbb"
                    verticalAlignment: Text.AlignVCenter
                    elide: Text.ElideRight
                }

                Rectangle{
                    height: 1
                    color: "#768bbb"
                    anchors.bottom: parent.bottom
                    anchors.bottomMargin: 0
                    width: parent.width
                }
            }

            Rectangle {
                id: enterButton
                y: 246
                width: parent.width
                height: 50
                activeFocusOnTab: true
                anchors.horizontalCenter: parent.horizontalCenter
                radius:12
                border.width: 0
                color: Theme.buttonColor

                Text {
                    id: tt
                    text: qsTr("Войти")
                    width: parent.width
                    height: parent.height
                    horizontalAlignment: Text.AlignHCenter
                    verticalAlignment: Text.AlignVCenter
                    font.family: golosRegularFont.name
                    font.pointSize: 14
                    color: "#ffffff"
                }

                MouseArea{
                    id: enterButtonMA
                    anchors.fill: parent
                    cursorShape: "PointingHandCursor"
                    hoverEnabled: true
                    onEntered: {
                        enterButton.color = Theme.buttonColorHover
                    }

                    onExited: {
                        enterButton.color = Theme.buttonColor
                    }

                    onClicked: {
                        Authorization.login(login.text, password.text, remember.isCheck, getServerId(server.currentIndex))
                    }
                }
            }

        }

    }

    Rectangle{
        id:loadinRect
        width: parent.width
        height: parent.height
        opacity: 0.5
        color: "#000000"
        anchors.fill: parent
        anchors.topMargin: 10
        anchors.bottomMargin: 9
        anchors.leftMargin: 7
        anchors.rightMargin: 11
        anchors.horizontalCenter: item1.horizontalCenter
        //visible: Manager.isAuthProcess
        visible: Manager.isAuthProcess

        BusyIndicator {
            id: busyIndicator
            anchors.verticalCenter: parent.verticalCenter
            anchors.horizontalCenter: parent.horizontalCenter
        }
    }
}
