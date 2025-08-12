#ifndef RECORD_H
#define RECORD_H

#include <QString>
#include <QJsonObject>

class Record
{
public:
  Record();

  QJsonObject jObject;
  int id;
  QString fileUrl;
  QString appVersion;
  int appVersionType;
  int appType;
  int crucially;
  QString description;
  int userId;
  QString dateTimeUpload;
  QString appVersionTypeTitle;
  QString appTypeTitle;
  QString extractPath(){ return appVersionTypeTitle.replace(" ", "_") + "_" + appVersion; }
  QString fileZipName(){ return appVersionTypeTitle.replace(" ", "_") + "_" + appVersion + ".zip"; }

  static Record *Parce(QJsonObject jDoc);

};

#endif // RECORD_H
