#ifndef FILEDATA_H
#define FILEDATA_H

#include "basemodel.h"
#include <QJsonObject>
#include <QObject>

namespace Api {
  namespace Models {

    struct FileModel : public BaseModel {
      FileModel();
      FileModel(QJsonObject jObject);
      FileModel(const FileModel &other);
      FileModel &operator=(const FileModel &other);
      FileModel(const FileModel &&other) noexcept;
      FileModel &operator=(const FileModel &&other) noexcept;

      const QString &fieldname() const;
      const QString &originalname() const;
      const QString &encoding() const;
      const QString &mimetype() const;
      const QString &destination() const;
      const QString &filename() const;
      const QString &path() const;
      const qint64 size() const;
      const QString &url() const;
      const QString &src() const;
      const QString &title() const;

    private:
      QString _fieldname;
      QString _originalname;
      QString _encoding;
      QString _mimetype;
      QString _destination;
      QString _filename;
      QString _path;
      qint64 _size;
      QString _url;
      QString _src;
      QString _title;
    };
  } // namespace Models
} // namespace Api
#endif // FILEDATA_H
