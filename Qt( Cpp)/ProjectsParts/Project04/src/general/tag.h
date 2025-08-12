#ifndef TAG_H
#define TAG_H

#include <QString>
#include <QJsonObject>

namespace General{

    class Tag
    {
    public:
        int id;
        QString name;
        QString description;

        static Tag Parse(QJsonObject jObj){
          Tag t;
          t.id = jObj.value("id").toInt();
          t.name = jObj.value("name").toString();
          t.description = jObj.value("description").toString();
          return t;
        }

    };
};

#endif // TAG_H
