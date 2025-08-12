import QtQuick
import QtQuick.Controls
import ReleaseState 1.0
import RunRelease 1.0
import Theme 1.0
import "../controls/."

Item {
    id: item1
    width: 1266
    height: 803

    property string tag: "Без тегов"

    Connections{
        target: ReleaseManager

        function onReleaseLoadSignal(){
            if(Theme.releasesListByTag){
                spawnMenuTags();
                spawnItemsTags();
            }else{
                spawnMenuBase();
                spawnItemsBase();
            }
            spawnItems();
        }
    }
    FontLoader{
        id: golosRegularFont
        source: "../static/fonts/Golos_Text_Regular.ttf"
    }

    onVisibleChanged: {
        if(visible)
            errorMessage.visible = ErrorController.getErrorList().length !== 0
        if(!item1.visible) return;
        if(Theme.releasesListByTag){
            spawnMenuTags();
        }else{
            spawnMenuBase();
        }
        spawnItems();
    }

    function spawnItems(){
        if(Theme.releasesListByTag){
            spawnItemsTags();
        }else{
            spawnItemsBase();
        }
    }

    function spawnMenuBase(){
        menuModel.clear();
        var tagsList = ReleaseManager.getTagsList();
        subOptions.visible = tagsList.length > 0
        tag = "Все";
        menuModel.append({name: "Все" });
        menuModel.append({name: "Избранное" });
    }

    function spawnMenuTags(){
        menuModel.clear();
        var tagsList = ReleaseManager.getTagsList();
        subOptions.visible = tagsList.length > 0

        if(tagsList.length <= 0) return;

        for(var i = 0 ; i < tagsList.length; i++){
            var item = tagsList[i];

            if(i == 0)
               tag = item;

            menuModel.append({name: item });
        }
        menuModel.append({name: "Без тегов" });
    }

    function clickItem(name){
        tag = name;
        for(var i = 0 ; i < menuModel.length; i++)
            menuModel[i].checkColor();
        spawnItems();
    }

    function spawnItemsTags(){
        listModel.clear();

        var useTag = tag == "Без тегов" ? "" : tag
        var releaseList = ReleaseManager.getVideowallList(isFavorite.isCheck,useTag);

        for(var i = 0 ; i < releaseList.length; i++){
            var release = releaseList[i];
            listModel.append({release: release });
        }
    }

    function spawnItemsBase(){
        listModel.clear();

        var isFavorite = tag == "Избранное" ? true : false
        var releaseList = ReleaseManager.getVideowallList(isFavorite,"");

        for(var i = 0 ; i < releaseList.length; i++){
            var release = releaseList[i];
            listModel.append({release: release });
        }
    }

    Rectangle{
        id: navPanel
        height: 78
        anchors.left: parent.left
        anchors.right: parent.right
        anchors.top: parent.top
        anchors.rightMargin: 70
        anchors.leftMargin: 70
        anchors.topMargin: 89
        color: "#00000000"

        ListView {
            //anchors.fill: parent
            id: releasesTags
            x: 17
            y: 18
            width: parent.width
            height: parent.height
            orientation: ListView.Horizontal
            spacing: 15
            interactive: false
            ScrollBar.horizontal: ScrollBar{
                active: false
                interactive: false
            }

            model: ListModel {
                id: menuModel
            }
            delegate: ReleasesMenuItem{
                title: name
                isSelected: tag == name
                onClickButton: {
                    clickItem(name);
                }
            }
        }
    }
    Rectangle{
        id: subOptions
        visible: false
        height: 50
        anchors.left: parent.left
        anchors.right: parent.right
        anchors.top: parent.top
        anchors.rightMargin: 70
        anchors.leftMargin: 70
        anchors.topMargin: navPanel.y+60
        color: "#00000000"

        CustomCheckBox{
            id: isFavorite
            visible: Theme.releasesListByTag
            width: 20
            height: 20
            margin: 3
            anchors.right: parent.right
            anchors.top: parent.top
            anchors.rightMargin: 70

            onOnChange: {
                spawnItems();
            }

            Text{
                x: 30
                y: 0
                text: qsTr("Избранное")
                color: "#768bbb"
                font.pointSize: 10
            }
        }
    }

    ErrorToggle{
        id: errorMessage
        visible: false
        height: 30
        width: parent.width - 106
        anchors.left: parent.horizontalCenter
        anchors.right: parent.right
        anchors.top: parent.top
        anchors.leftMargin: -577
        anchors.rightMargin: 56
        anchors.topMargin: 167
    }
    ScrollView {
        id: scrollView
        anchors.fill: parent
        enabled: true
        focusPolicy: Qt.NoFocus
        anchors.rightMargin: 50
        anchors.leftMargin: 56
        anchors.bottomMargin: 59
        anchors.topMargin: (Theme.releasesListByTag ? 190 : 170) + (errorMessage.visible ? errorMessage.height + 10 : 0)
        clip:true
        ScrollBar.horizontal.policy: ScrollBar.AlwaysOff

        // ScrollBar.vertical: ScrollBar {
        //         policy: ScrollBar.AlwaysOn
        //         size: 0.3
        //         position: 0.2
        //         active: true
        //         width: 7
        //         orientation: Qt.Vertical
        //         anchors.right: parent.right
        //         anchors.top: parent.top
        //         anchors.bottom: parent.bottom
        //         anchors.rightMargin: 0
        //         anchors.topMargin: 0
        //         anchors.bottomMargin: 0
        //         contentItem: Rectangle {
        //             implicitWidth: 6
        //             //implicitHeight: 100
        //             radius: width / 2
        //             color: "#c2c9da"
        //             width: parent.width
        //             height: 10
        //         }
        //         background: Rectangle{
        //             color: "red"
        //             radius: width / 2
        //             width: parent.width
        //         }

        //     }

        ListView {
            id: listView
            width: scrollView.width != null ? scrollView.width : 0
            height: parent.height
            spacing: 15
            boundsBehavior: Flickable.StopAtBounds

            model: ListModel {
                id: listModel
            }
            delegate: ReleaseItem{
                releaseRecord: release
            }
        }
    }
}
