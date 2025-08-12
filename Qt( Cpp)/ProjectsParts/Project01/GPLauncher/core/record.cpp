#include "record.h"

Record::Record()
{

}

Record *Record::Parce(QJsonObject jDoc)
{

  Record *release = new Record();
  release->jObject = jDoc;
  release->id = jDoc.value("id").toInt();
  release->fileUrl = jDoc.value("file").toString();
  release->appVersion = jDoc.value("app_version").toString();
  release->appVersionType = jDoc.value("app_version_type").toInt();
  release->appType = jDoc.value("app_type").toInt();
  release->crucially = jDoc.value("crucially").toInt();
  release->description = jDoc.value("descr").toString();
  release->userId = jDoc.value("user_id").toInt();
  release->dateTimeUpload = jDoc.value("date_time_upload").toString();
  release->appVersionTypeTitle = jDoc.value("app_version_type_title").toString();
  release->appTypeTitle = jDoc.value("app_type_title").toString();

  return release;
}
