#include <QSettings>
#include <QString>

#include <iostream>

void generateIni(std::string filename
                 , std::string path
                 , std::string runFile
//                 , std::string type
//                 , std::string statusVersion
                 , std::string version
                 , std::string strim
                 , std::string noteUrl){

    QSettings *setting = new QSettings(QString::fromStdString(path) + "/" + QString::fromStdString(filename), QSettings::IniFormat);

    setting->setValue("name",QString::fromStdString(strim));
//    setting->setValue("type",QString::fromStdString(type));
    setting->setValue("runFile",QString::fromStdString(runFile));
//    setting->setValue("statusVersion",QString::fromStdString(statusVersion));
    setting->setValue("version",QString::fromStdString(version));
    setting->setValue("note",QString::fromStdString(noteUrl));
    setting->sync();

}
