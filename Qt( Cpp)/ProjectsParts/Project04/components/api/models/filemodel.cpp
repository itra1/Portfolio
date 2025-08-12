#include "filemodel.h"

namespace Api {
  namespace Models {

    FileModel::FileModel() {}

    FileModel::FileModel(QJsonObject jObject) : BaseModel(jObject) {
      if (_jObject.contains("fieldname"))
        _fieldname = _jObject.value("fieldname").toString();
      if (_jObject.contains("originalname"))
        _originalname = _jObject.value("originalname").toString();
      if (_jObject.contains("encoding"))
        _encoding = _jObject.value("encoding").toString();
      if (_jObject.contains("mimetype"))
        _mimetype = _jObject.value("mimetype").toString();
      if (_jObject.contains("destination"))
        _destination = _jObject.value("destination").toString();
      if (_jObject.contains("filename"))
        _filename = _jObject.value("filename").toString();
      if (_jObject.contains("size"))
        _size = _jObject.value("size").toInteger();
      if (_jObject.contains("url"))
        _url = _jObject.value("url").toString();
      if (_jObject.contains("src"))
        _src = _jObject.value("src").toString();
      if (_jObject.contains("title"))
        _title = _jObject.value("title").toString();
    }

    FileModel::FileModel(const FileModel &other)
        : BaseModel{other}, _fieldname{other.fieldname()},
          _originalname{other.originalname()}, _encoding{other.encoding()},
          _mimetype{other.mimetype()}, _destination{other.destination()},
          _filename{other.filename()}, _size{other.size()}, _url{other.url()},
          _src{other.src()}, _title{other.title()} {}

    FileModel &FileModel::operator=(const FileModel &other) {
      BaseModel::operator=(other);
      _fieldname = other.fieldname();
      _originalname = other.originalname();
      _encoding = other.encoding();
      _mimetype = other.mimetype();
      _destination = other.destination();
      _filename = other.filename();
      _size = other.size();
      _url = other.url();
      _src = other.src();
      _title = other.title();

      return *this;
    }

    FileModel::FileModel(const FileModel &&other) noexcept
        : BaseModel{other}, _fieldname{other.fieldname()},
          _originalname{other.originalname()}, _encoding{other.encoding()},
          _mimetype{other.mimetype()}, _destination{other.destination()},
          _filename{other.filename()}, _size{other.size()}, _url{other.url()},
          _src{other.src()}, _title{other.title()} {}

    FileModel &FileModel::operator=(const FileModel &&other) noexcept {
      BaseModel::operator=(other);
      _fieldname = other.fieldname();
      _originalname = other.originalname();
      _encoding = other.encoding();
      _mimetype = other.mimetype();
      _destination = other.destination();
      _filename = other.filename();
      _size = other.size();
      _url = other.url();
      _src = other.src();
      _title = other.title();

      return *this;
    }

    const QString &FileModel::fieldname() const { return _fieldname; }

    const QString &FileModel::originalname() const { return _originalname; }

    const QString &FileModel::encoding() const { return _encoding; }

    const QString &FileModel::mimetype() const { return _mimetype; }

    const QString &FileModel::destination() const { return _destination; }

    const QString &FileModel::filename() const { return _filename; }

    const QString &FileModel::path() const { return _path; }

    const qint64 FileModel::size() const { return _size; }

    const QString &FileModel::url() const { return _url; }

    const QString &FileModel::src() const { return _src; }

    const QString &FileModel::title() const { return _title; }
  } // namespace Models

} // namespace Api
