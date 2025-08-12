#ifndef MATERIALACTION_H
#define MATERIALACTION_H

#include <QObject>
#include <QJsonObject>
#include "action.h"

namespace CnpSocket
{
  struct MaterialAction : Action
  {
    MaterialAction(QJsonObject jObject);

  private:
    qint64 _materialId;
    qint64 _areaId;
    qint64 _episodeId;
    qint64 _statusContentId;
    QString _uuid;
  };

  inline MaterialAction::MaterialAction(QJsonObject jObject)
      : Action(jObject)
  {
    _materialId = jObject.value("materialId").toInteger();
    _areaId = jObject.value("areaId").toInteger();
    _episodeId = jObject.value("episodeId").toInteger();
    _statusContentId = jObject.value("statusContentId").toInteger();
    _uuid = jObject.value("uuid").toString();
  }
}

#endif