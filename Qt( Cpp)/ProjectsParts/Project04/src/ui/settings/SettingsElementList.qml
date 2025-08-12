import QtQuick
import "../controls/."
import SettingsItem 1.0

Item {
    id: baseItem
    property SettingsItem item;

    property string elementType: "";

    signal itemChange();

    enabled: {
        elementType = item.type();
        return true;
    }

    SettingsToggle{
        visible: elementType == "toggle" || elementType == "toggleRegedit"
        width: parent.width
        height: 50
        title: item.name()
        tooltipString: item.toolType()
        isEnable: item.isEnable();
        isChecked: false
        onValueChange: {
            item.setValueAsBool(value);
        }

        onVisibleChanged: {
            if(visible){
                //title = item.name();
                isChecked = item.valueAsBool();
                //tooltipString = item.toolType();
                baseItem.height = itemHeight;
            }
        }
    }

    SettingsInputField{
        visible: elementType == "inputInt"
        width: parent.width
        height: 45
        fieldWidth: 200
        title: ""
        echoMode: TextInput.Normal
        inputMethodHit: Qt.ImhPreferNumbers
        tooltipString: item.toolType()
        onValueChange: {
            item.setValueAsInt(value);
            itemChange();
        }
        onVisibleChanged: {
            if(visible){
                title = item.name();
                value = item.valueAsInt();
                baseItem.height = itemHeight;
            }
        }
    }

    SettingsInputField{
        visible: elementType == "inputFloat"
        width: parent.width
        height: 45
        fieldWidth: 200
        title: ""
        echoMode: TextInput.Normal
        inputMethodHit: Qt.ImhNone
        tooltipString: item.toolType()
        onValueChange: {
            item.setValueAsFloat(value);
            itemChange();
        }
        onVisibleChanged: {
            if(visible){
                title = item.name();
                value = item.valueAsFloat().toFixed(2);
                baseItem.height = itemHeight;
            }
        }
    }

    SettingsInputField{
        visible: elementType == "inputString"
        width: parent.width
        height: 45
        title: ""
        echoMode: TextInput.Normal
        inputMethodHit: Qt.ImhNone
        tooltipString: item.toolType()
        onValueChange: {
            item.setValueAsString(value);
            itemChange();
        }
        onVisibleChanged: {
            if(visible){
                title = item.name();
                value = item.valueAsString();
                baseItem.height = itemHeight;
            }
        }
    }

    SettingsInputField{
        visible: elementType == "inputPassword"
        width: parent.width
        height: 45
        title: ""
        inputMethodHit: Qt.ImhNone
        echoMode: TextInput.Password
        tooltipString: item.toolType()
        onValueChange: {
            item.setValueAsString(value);
            itemChange();
        }
        onVisibleChanged: {
            if(visible){
                title = item.name();
                value = item.valueAsString();
                baseItem.height = itemHeight;
            }
        }
    }

    SettingsInput{
        visible: elementType == "filePath"
        height: 45
        width: parent.width-40
        title: ""
        inputMethodHit: Qt.ImhNone
        tooltipString: item.toolType()
        useFileDialog: true
        onValueChange: {
            item.setValueAsString(value);
            itemChange();
        }
        onVisibleChanged: {
            if(visible){
                title = item.name();
                aliasValue = item.valueAsString();
                fileDialogtitle = item.paramAsString("fileDialogTitle");
                fileDialogFilter = item.paramAsString("fileDialogFilter");
                baseItem.height = itemHeight;
            }
        }
    }

}
