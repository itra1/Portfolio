#include "basemodel.h"

namespace Api {
  namespace Models {

    BaseModel::BaseModel() {}

    BaseModel::BaseModel(QJsonObject jObject) {
      _jObject = jObject;

      if (jObject.contains("id"))
        _id = jObject.value("id").toInteger();
      if (jObject.contains("destination"))
        _description = jObject.value("destination").toString();
    }

    BaseModel::BaseModel(const BaseModel &other) {}

    BaseModel &BaseModel::operator=(const BaseModel &other) {
      _id = other.id();
      _description = other.description();
      return *this;
    }

    BaseModel::BaseModel(const BaseModel &&other) noexcept
        : _id{other.id()}, _description{other.description()} {}

    BaseModel &BaseModel::operator=(const BaseModel &&other) noexcept {
      _id = other.id();
      _description = other.description();
      return *this;
    }

    const qint64 BaseModel::id() const { return _id; }

    void BaseModel::setId(qint64 newId) { _id = newId; }

    const QString &BaseModel::description() const { return _description; }

    void BaseModel::setDescription(const QString &newDescription) {
      _description = newDescription;
    }

  } // namespace Models
} // namespace Api
