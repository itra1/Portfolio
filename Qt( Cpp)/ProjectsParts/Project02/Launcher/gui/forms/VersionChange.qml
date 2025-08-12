import QtQuick 2.11
import QtQuick.Controls 2.4

Item {

    onVisibleChanged: {
        if(this.visible){

            var versionsList = mainModel.getGameVersionsList();

            selectVersion.visible = versionsList.length > 1;
            versionOne.visible = !selectVersion.visible;

            if(versionOne.visible){
                versionOneText.text = versionsList[0];
            }else{
                selectVersion.model = versionsList;
                selectVersion.currentIndex = mainModel.getGameVersionsIndex();
            }

        }
    }

    Rectangle{
        id: versionOne
        width: selectVersion.width
        height: selectVersion.height
        anchors.top: selectVersion.anchors.top
        anchors.topMargin: selectVersion.anchors.topMargin
        color: "#2e2249"

        Text{
            id: versionOneText
            leftPadding: 29
            height: parent.height
            text: ""
            font: selectVersion.font
            color: "white"
            verticalAlignment: Text.AlignVCenter
            horizontalAlignment: Text.AlignLeft
            elide: Text.ElideRight

        }

//        onHoveredChanged: {
//            mainWindow.showInfo(this,qsTr("Version"));
//        }


    }

    ComboBox {
        id: selectVersion
        width: parent.width
        height: parent.height
        anchors.top: parent.top
        anchors.topMargin: 0
        font.pixelSize: 15

        onHoveredChanged: {
            mainWindow.showInfo(this,qsTr("Version"));
        }

        onActivated: {
            mainModel.setGameVersionsView(index);
        }

        contentItem: Text {
            leftPadding: 29
            text: selectVersion.displayText
            font: selectVersion.font
            color: "white"
            verticalAlignment: Text.AlignVCenter
            horizontalAlignment: Text.AlignLeft
            elide: Text.ElideRight
        }

        background: Rectangle{
            width: parent.width
            height: parent.height
            color: "#2e2249"
        }

        indicator:Image{
            width:14; height:7;
            anchors.top: parent.top
            anchors.topMargin: 30
            anchors.right: parent.right
            anchors.rightMargin: 22
            horizontalAlignment:Image.AlignRight
            source:"qrc:///img/downArrowW.png"
        }

        delegate: ItemDelegate{
            id:itemDlgt
            width: selectVersion.width
            height:selectVersion.height *0.8

            contentItem: Text {
                id:textItem
                text: modelData
                color: hovered?"#ffffff":"#7d768f"
                font: selectVersion.font
                elide: Text.ElideRight
                leftPadding: 15
                verticalAlignment: Text.AlignVCenter
                horizontalAlignment: Text.AlignLeft
            }

            background: Rectangle {
                color:itemDlgt.hovered?"#3e3357":"transparent";
                anchors.left: itemDlgt.left
                anchors.leftMargin: 0
                width:itemDlgt.width-2
            }
        }

        popup: Popup {
            id:selectVersionPopup
            y: selectVersion.height - 1
            width: selectVersion.width
            height:selectVersion.contentItem.implicitHeigh
            padding: 0

            contentItem: ListView {
                id:listView
                implicitHeight: contentHeight
                model: selectVersion.popup.visible ? selectVersion.delegateModel : null
                ScrollIndicator.vertical: ScrollIndicator { }
            }

            background: Rectangle {
                color: "#2e2249"
            }
        }
    }
}



/*##^## Designer {
    D{i:0;autoSize:true;height:480;width:640}
}
 ##^##*/
