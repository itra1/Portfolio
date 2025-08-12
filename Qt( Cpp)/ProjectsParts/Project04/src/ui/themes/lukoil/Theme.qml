pragma Singleton
import QtQuick

Item {
    property string disableColor: "#666666"
    property string disableTransparentColor: "#66666666"
    property string mainTextColor: "#ffffff"
    property string textColor: "#768bbb"
    property string fieldsFill: "#1aa2d1f2"
    property string inputsBorder: "#768bbb"
    //! Стандартная кнопка
    property string buttonColor: "#2567C9"
    //! Стандартная кнопка Hover
    property string buttonColorHover: "#1a52a6"
    property int settingsTitleWidth: 300
    //! Настройки: показывать список серверов
    property bool settingsVisibleServers: true
    //! картинки: Лого в левом нижнем углу
    property string picturelogo: "/forms/LauncherQml/src/ui/themes/lukoil/img/Logo.png"
    //! Дробитьсписок релизов на теги
    property bool releasesListByTag: false
}