#ifndef API_MODELS_RELEASE_H
#define API_MODELS_RELEASE_H
#include "basemodel.h"
#include "filemodel.h"
#include <QJsonObject>
#include <QString>

namespace Api {
  namespace Models {

    struct ReleaseModel : BaseModel {

      ReleaseModel(QJsonObject jObject);

      const QString &type() const;
      const QString &version() const;
      const QString &checksum() const;
      const QDateTime &createAt() const;
      const FileModel &file() const;

    private:
      QString _type;
      QString _version;
      QString _checksum;
      QDateTime _createAt;
      FileModel _file{};
      QList<int> tagsIds{};
    };

  } // namespace Models
} // namespace Api

#endif // API_MODELS_RELEASE_H
