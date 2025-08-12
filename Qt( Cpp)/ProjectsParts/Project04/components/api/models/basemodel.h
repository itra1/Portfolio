#ifndef BASEMODEL_H
#define BASEMODEL_H
#include <QJsonObject>

namespace Api {
  namespace Models {

    struct BaseModel {
      BaseModel();
      BaseModel(QJsonObject jObject);
      BaseModel(const BaseModel& other);
      BaseModel& operator=(const BaseModel& other);
      BaseModel(const BaseModel&& other) noexcept;
      BaseModel& operator=(const BaseModel&& other) noexcept;

      const qint64 id() const;
      void setId(qint64 newId);

      const QString& description() const;
      void setDescription(const QString& newDescription);

    protected:
      qint64 _id;
      QString _description;
      QJsonObject _jObject;
    };
  } // namespace Models
} // namespace Api

#endif // BASEMODEL_H
