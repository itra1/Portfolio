#include "releasemodel.h"
#include "filemodel.h"

namespace Api {
  namespace Models {

    ReleaseModel::ReleaseModel(QJsonObject jObject) : BaseModel(jObject) {
      if (_jObject.contains("version"))
        _version = _jObject.value("version").toString();
      if (_jObject.contains("checksum"))
        _checksum = _jObject.value("checksum").toString();
      if (_jObject.contains("type"))
        _type = _jObject.value("type").toString();
      if (_jObject.contains("createdAt"))
        _createAt = QDateTime::fromString(
            _jObject.value("createdAt").toString(), Qt::ISODate);
      if (_jObject.contains("file"))
        _file = FileModel(_jObject.value("file").toObject());
    }

    const QString &ReleaseModel::type() const { return _type; }

    const QString &ReleaseModel::checksum() const { return _checksum; }

    const QDateTime &ReleaseModel::createAt() const { return _createAt; }

    const FileModel &ReleaseModel::file() const { return _file; }

    const QString &ReleaseModel::version() const { return _version; }

  } // namespace Models
} // namespace Api
