import QtQuick 2.11
import QtQuick.Controls 2.4

Item {
    id: footerLoginBlock

    FontLoader{
        id: robotoRegularFont
        source: "qrc:///fonts/Roboto-Regular.ttf"
    }

    Button{
        id: loginButton
        width: 192;
        height: 64
        rightPadding: 30
        leftPadding: 30
        font.capitalization: Font.AllUppercase

        background: Rectangle{
            width: parent.width
            height: parent.height
            color: loginButton.down ? "#c800fff2" : "#00fff2"
        }

        contentItem: Text {
            id: loginButtonText
            text: qsTr("LOGIN")
            font.capitalization: Font.AllUppercase
            font.pixelSize: 28
            verticalAlignment: Text.AlignVCenter
            horizontalAlignment: Text.AlignHCenter
            font.family: robotoRegularFont.name
        }
        onClicked: {
            model.logIn();
        }
    }

}
