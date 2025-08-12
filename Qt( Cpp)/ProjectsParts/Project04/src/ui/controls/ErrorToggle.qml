import QtQuick
import QtQuick.Controls

Rectangle {
    id: main
    height: 30
    color: "#30e29d9d"
    border.color: "#f67777"
    border.width: 1
    visible: ErrorController.getErrorList().length > 0

    property int index: 0

    onVisibleChanged: {
        if(visible){
            index = -1
            toggleError();
        }
    }

    function toggleError(){
        var errorList = ErrorController.getErrorList();
        index++;
        if(index >= errorList.length)
            index = 0;
        if(errorList.length === 0){
            counterLabel.text = "";
            errorMessage.text = '';
        }else{
            counterLabel.text = (index+1) + '/' + errorList.length;
            errorMessage.text = errorList[index];
        }
    }

    FontLoader{
        id: golosRegularFont
        source: "../static/fonts/Golos_Text_Regular.ttf"
    }

    Rectangle{
        id: counter
        height: parent.height
        width: 50
        color: "#00000000"

        Label{
            id: counterLabel
            anchors.fill: parent
            font.pixelSize: 15
            horizontalAlignment: Text.AlignHCenter
            verticalAlignment: Text.AlignVCenter
            color: "#ff2424"
            text: "5/5"
        }
    }

    Rectangle{
        id: error
        height: parent.height
        color: "#00000000"
        anchors.right: parent.right
        anchors.rightMargin: 0
        width: parent.width-counter.width-5

        Label{
            id: errorMessage
            anchors.fill: parent
            font.pixelSize: 13
            font.family: golosRegularFont.name
            minimumPixelSize: 8
            horizontalAlignment: Text.AlignHCenter
            verticalAlignment: Text.AlignVCenter
            color: "#ff2424"
            wrapMode: Text.WordWrap
            textFormat: Text.RichText
            elide: Text.ElideNone
        }
    }

    MouseArea{
        anchors.fill: parent
        onClicked: {
            toggleError();
        }
    }
}

/*##^##
Designer {
    D{i:0;formeditorZoom:2}
}
##^##*/
