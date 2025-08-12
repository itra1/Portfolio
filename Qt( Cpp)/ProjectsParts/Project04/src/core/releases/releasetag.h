#ifndef RELEASETAG_H
#define RELEASETAG_H

#include <QObject>
namespace Core {
class ReleaseTag : public QObject
{
    Q_OBJECT
public:
    explicit ReleaseTag(QObject *parent = nullptr);

    //! Идентификатор
    int id() const;
    //! Имя
    QString name() const;
    //! Описание
    QString description() const;

    void parse(QJsonObject jObj);

private:
    int _id;
    QString _name;
    QString _description;
};
};     // namespace Core
#endif // RELEASETAG_H
