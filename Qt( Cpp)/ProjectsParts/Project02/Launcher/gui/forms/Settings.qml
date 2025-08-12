import QtQuick 2.11
import QtQuick.Dialogs 1.3
import QtQuick.Controls 2.4

Item {

    id: settingLayer
    width: 1000
    height: 700

    onVisibleChanged: {
        if(settingLayer.visible){
            automaticUpdateSetting.checkState = model.isAutomaticUpdate ? Qt.Checked : Qt.Unchecked
            p2pEnableSetting.checkState = model.isP2pDownload ? Qt.Checked : Qt.Unchecked

            languageSelected.model = mainModel.getLanguageList();
            languageSelected.currentIndex = mainModel.getLanguageIndex();
            checkGameCacheButtonRect.border.color = mainModel.isProcess ? "#9b9b9b" : "#00fff2"
            checkGameCacheButtonText.color = mainModel.isProcess ? "#9b9b9b" : "#00fff2"
        }
    }
    Connections{
        target: mainModel

        onOnInstallPathChange:{
            installPathField.text = mainModel.installationPath;
        }
    }

    Rectangle{
        id: backGround
        width: parent.width
        height: parent.height
        color: "#2d2d2d"
        radius: 8

    }

    Button{
        id: noProclickButton
        width: parent.width
        height: parent.height
        background: Image{
            width: parent.width
            height: parent.heigh
        }
    }

    Button {
        id: settingsCloseButton
        anchors.left: parent.left
        anchors.top: parent.top
        anchors.topMargin: 171
        anchors.leftMargin: 86
        spacing: 5

        contentItem: Image{
            width: settingsCloseButton.width - 16
            height: settingsCloseButton.height - 12
            source: "qrc:/img/arrow.png"
        }

        background: Rectangle{
            width: settingsCloseButton.width
            height: settingsCloseButton.height
            color: "#ffffff"
            opacity: settingsCloseButton.down ? 0.5 : 0
        }

        onClicked: {
            model.settingsButton();
        }

        onHoveredChanged: {
            mainWindow.showInfo(this,qsTr("Back"));
        }

    }

    Item {
        id: content
        anchors.bottomMargin: 96
        anchors.topMargin: 163
        anchors.leftMargin: 212
        anchors.fill: parent

        Text{
            color: "#ffffff"
            text: qsTr("Game settings")
            font.pixelSize: 28

        }

        Button {
            id: checkGameCacheButton
            x: 0
            height: 32
            spacing: 3
            clip: false
            anchors.top: parent.top
            anchors.topMargin: 65

            background: Rectangle{
                id: checkGameCacheButtonRect
                width: parent.width
                height: parent.height
                color: checkGameCacheButton.down ? "#3000fff2" : "transparent"
                border.color:  mainModel.isProcess ? "#9b9b9b" : "#00fff2"
                border.width: 1
            }

            contentItem: Text {
                id: checkGameCacheButtonText
                text: qsTr("Verify integrity of game files")
                font.pixelSize: 15
                verticalAlignment: Text.AlignVCenter
                color: "#00fff2"
            }
            onClicked: {
                model.checkGameCacheButton();
            }

        }

        Rectangle{
            id: languageBlock
            anchors.left: parent.left
            anchors.leftMargin: 327

            Text{
                color: "#ffffff"
                text: qsTr("Language")
                font.pixelSize: 28
            }

            ComboBox {
                id: languageSelected
                width: 240
                height: 32
                anchors.top: parent.top
                anchors.topMargin: 65
                font.pixelSize: 15

                onActivated: {
                    mainModel.setLanguageView(index);
                }

                contentItem: Text {
                    leftPadding: 10
                    text: languageSelected.displayText
                    font: languageSelected.font
                    color: "white"
                    verticalAlignment: Text.AlignVCenter
                    horizontalAlignment: Text.AlignLeft
                    elide: Text.ElideRight
                }

                background: Rectangle{
                    width: parent.width
                    height: parent.height
                    color: languageSelected.down ? "#3000fff2" : "transparent"
                    border.color:  "#757575"
                    border.width: 1
                }

                indicator:Image{
                    width:14; height:7;
                    anchors.top: parent.top
                    anchors.topMargin: 12
                    anchors.right: parent.right
                    anchors.rightMargin: 11
                    horizontalAlignment:Image.AlignRight
                    source:"qrc:///img/downArrow.png"
                }

                delegate: ItemDelegate{
                    id:itemDlgt
                    width: languageSelected.width
                    height:32

                    contentItem: Text {
                        padding: 0
                        id:textItem
                        text: modelData
                        color: hovered?"#00fff2":"#ffffff"
                        font: languageSelected.font
                        elide: Text.ElideRight
                        verticalAlignment: Text.AlignVCenter
                        horizontalAlignment: Text.AlignLeft
                    }

                    background: Rectangle {
                        color:itemDlgt.hovered?"#757575":"transparent";
                        anchors.left: itemDlgt.left
                        anchors.leftMargin: 0
                        width:itemDlgt.width-2
                    }

                }

                popup: Popup {
                    id:languageSelectedPopup
                    y: languageSelected.height - 1
                    width: languageSelected.width
                    height:languageSelected.contentItem.implicitHeigh
                    padding: 0

                    contentItem: ListView {
                        id:listView
                        implicitHeight: contentHeight
                        model: languageSelected.popup.visible ? languageSelected.delegateModel : null
                        //ScrollIndicator.vertical: ScrollIndicator { }
                    }

                    background: Rectangle {
                        color: "#555555"
                    }
                }

            }

        }

        Item{
            id: element
            anchors.top: parent.top
            anchors.topMargin: 303-content.anchors.topMargin
            width: content.width
            height: 120

            Text {
                color: "#ffffff"
                text: qsTr("Application settings")
                font.pixelSize: 28
            }

            CheckBox {
                id: p2pEnableSetting
                y:  61
                width: 211
                height: 19
                anchors.left: parent.left
                anchors.leftMargin: 27
                nextCheckState: function() {
                    model.isP2pDownload = checkState !== Qt.Checked;
                    if (checkState === Qt.Checked){
                        return Qt.Unchecked
                    }else{
                        return Qt.Checked
                    }

                }
                indicator: Rectangle{
                    width: 12
                    height: 12
                    color: "transparent"
                    anchors.top: parent.top
                    anchors.topMargin: 4
                    border.color: model.isP2pDownload ? "#00fff2" : "#ffffff"
                    Image{
                        visible: model.isP2pDownload
                        x: 2
                        y: 3
                        source: "qrc:///img/icon-check.svg"

                    }
                }
                Label{
                    color: "#ffffff"
                    text: qsTr("Turn on peer-to-peer upload")
                    font.pixelSize: 15
                    anchors.left: parent.left
                    anchors.leftMargin: 23
                }
            }

            CheckBox {
                id: automaticUpdateSetting
                y: 96
                width: 211
                height: 24
                anchors.left: parent.left
                anchors.leftMargin: 27
                nextCheckState: function() {
                    model.isAutomaticUpdate = checkState !== Qt.Checked;
                    if (checkState === Qt.Checked){
                        return Qt.Unchecked
                    }else{
                        return Qt.Checked
                    }
                }

                indicator: Rectangle{
                    width: 12
                    height: 12
                    color: "transparent"
                    anchors.top: parent.top
                    anchors.topMargin: 4
                    border.color: model.isAutomaticUpdate ? "#00fff2" : "#ffffff"
                    Image{
                        visible: model.isAutomaticUpdate
                        x: 2
                        y: 3
                        source: "qrc:///img/icon-check.svg"

                    }
                }
                Label{
                    color: "#ffffff"
                    text: qsTr("Update launcher and game client automatically")
                    font.pixelSize: 15
                    anchors.left: parent.left
                    anchors.leftMargin: 23
                }


            }
        }


        Item{
            id: directionBlock
            anchors.top: parent.top
            anchors.topMargin: 460-content.anchors.topMargin
            width: content.width
            height: 120

            Text {
                id: element1
                color: "#9effffff"
                text: qsTr("Game files location")
                font.pixelSize: 11
                font.capitalization: Font.AllUppercase
            }

            TextField {
                id: installPathField
                x: 0
                y: 40
                width: 424
                height: 32
                autoScroll: true
                text: ""
                renderType: Text.NativeRendering
                font.pixelSize: 15
                hoverEnabled: true
                selectByMouse: true
                selectedTextColor: "#00fff2"
                selectionColor: "#50ffffff"
                wrapMode: TextInput.NoWrap

                background: Rectangle{
                    width: 424
                    height: 32
                    color: "#00ffffff"
                    border.color: model.isCorrectPath ? "#66ffffff" : "#fb7780"
                }

                onVisibleChanged: {
                    if(installPathField.visible){
                        installPathField.text = model.installationPath;
                    }
                }

                onTextChanged: {
                    model.checkInstallPath(installPathField.text);
                }

                color: "#e8e8e8"
            }

            Button {
                id: choseInstallPath
                x: 443
                y: 40
                height: 32
                spacing: 3
                clip: false
                anchors.top: parent.top
                anchors.topMargin: 40

                background: Rectangle{
                    width: parent.width
                    height: parent.height
                    color: choseInstallPath.down ? "#3000fff2" : "transparent"
                    border.color:  "#00fff2"
                    border.width: 1
                }

                contentItem: Text {
                    id: name1
                    text: qsTr("Change location")
                    font.pixelSize: 15
                    verticalAlignment: Text.AlignVCenter
                    color: "#00fff2"
                }

                onClicked: {
                    model.installPathFileDialog();
                }

            }

        }


    }

    Rectangle{
        width: 1000
        id: footerRect
        height: 96
        color: 'transparent'
        radius: 8
        anchors.bottom: parent.bottom
        anchors.bottomMargin: 0
        transformOrigin: Item.Bottom
        border.width: 0
        clip: true

        Rectangle {
            id: footerRectClipped
            x: 0
            y: 0
            width: parent.width
            height: 96
            border.width: 0
            color: "#1f1f1f"
            radius: 8

        }

        Button {
            id: buttonRestoreSettings
            height: 32
            anchors.bottom: parent.bottom
            anchors.bottomMargin: 32
            anchors.left: parent.left
            anchors.leftMargin: 37
            spacing: 3
            clip: false

            background: Rectangle{
                width: parent.width
                height: parent.height
                color: buttonRestoreSettings.down ? "#509b9b9b" : "transparent"
                border.color: buttonRestoreSettings.down ? "#000000" : "#9b9b9b"
                border.width: 1
            }

            contentItem: Text {
                id: titleSettings
                text: qsTr("Restore default settings")
                font.pixelSize: 15
                verticalAlignment: Text.AlignVCenter
                color: buttonRestoreSettings.down ? "#000000" : "#9b9b9b"
            }
            onClicked: {
                model.restoreDefaultSettings();
            }

        }

        Button {
            id: buttonSaveSettings
            height: 32
            rightPadding: 20
            leftPadding: 20
            font.wordSpacing: -0.2
            anchors.right: parent.right
            anchors.rightMargin: 32
            anchors.bottom: parent.bottom
            anchors.bottomMargin: 32
            spacing: 3
            clip: false

            background: Rectangle{
                width: parent.width
                height: parent.height
                color: model.isDirty ? (buttonSaveSettings.down ? "#3000fff2" : "transparent" )
                                                 : (buttonSaveSettings.down ? "#9900fff2" : "#00fff2" )
                border.color: "#00fff2"
                border.width: 1
            }

            contentItem: Text {
                id: titleSaveSettings
                text: model.isDirty ? qsTr("Save") : qsTr("Done")
                font.pixelSize: 15
                verticalAlignment: Text.AlignVCenter
                color: model.isDirty ? "#00fff2" : "#000000"
            }
            onClicked: {
                model.saveButton();
            }

        }

        Rectangle{
            id: rectangle
            visible: !model.isDirty
            x: 743
            y: 32
            width: 143
            height: 32
            anchors.bottom: parent.bottom
            anchors.bottomMargin: 32
            anchors.right: parent.right
            anchors.rightMargin: 142
            color: "transparent"

            Text{
                id: saveText
                color: "#00fff2"
                text: qsTr("Changes saved")
                anchors.top: parent.top
                anchors.topMargin: 7
                verticalAlignment: Text.AlignVCenter
                horizontalAlignment: Text.AlignRight
                anchors.right: parent.right
                anchors.rightMargin: 0
                font.pixelSize: 15
                font.weight: Font.Normal

            }

            Image{
                width: 18
                height: 12
                anchors.right: parent.right
                anchors.rightMargin: saveText.width + 12
                anchors.top: parent.top
                anchors.topMargin: 10
                source: "qrc:///img/icon-check-big.svg"

            }
        }
    }

}
