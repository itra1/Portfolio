import QtQuick;
import Theme 1.0

Rectangle{
  id: updateButton
  width: 100
  height: 34
  radius: 3
  color: Theme.buttonColor

  property string tooltipString

  FontLoader{
      id: golosRegularFont
      source: "../static/fonts/Golos_Text_Regular.ttf"
  }

  Text{
    id: title
    anchors.fill: parent
    text: qsTr("Обновить")
    verticalAlignment: Text.AlignVCenter
    horizontalAlignment: Text.AlignHCenter
    font.family: golosRegularFont.name
    font.pointSize: 12
    color: "#ffffff"
  }

  Tooltip{
    id: thisTooltip
    x: 0
    y: updateButton.height
    title: tooltipString
  }

  MouseArea{
      id: mouseArea
      anchors.fill: parent
      cursorShape: "PointingHandCursor"
      hoverEnabled: true
      onClicked:{
          UpdaterManager.update();
      }
      onEntered: {
          if(tooltipString.length > 0){
              thisTooltip.popupVisible = true;
          }
      }
      onExited: {
          if(tooltipString.length > 0){
              thisTooltip.popupVisible = false;
          }
      }
  }

}
