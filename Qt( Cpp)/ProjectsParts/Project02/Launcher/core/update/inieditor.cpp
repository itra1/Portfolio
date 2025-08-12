#include "inieditor.h"

using namespace Core;

// Edit ini info
IniEditor::IniEditor(QString filePath, QObject *parent) : QObject(parent),
    m_settings(new QSettings(filePath, QSettings::IniFormat))
{

}

void IniEditor::write(QString key, int value)
{
    m_settings->setValue(key,value);
}

void IniEditor::write(QString key, double value)
{
    m_settings->setValue(key,value);
}

void IniEditor::write(QString key, QString value)
{
    m_settings->setValue(key,value);
}

void IniEditor::write(QHash<QString, QVariant> value)
{
    for (QHash<QString, QVariant>::iterator it = value.begin(); it != value.end(); ++it)
    {
        m_settings->setValue(it.key(),it.value());
    }
}

bool IniEditor::hash(QString key)
{
    return m_settings->contains(key);
}

QHash<QString, QVariant> IniEditor::read()
{
    QHash<QString, QVariant> hash;

    for(QString key : m_settings->allKeys() ){
        m_settings->setValue(key,m_settings->value(key));
    }
    return hash;
}

QVariant IniEditor::read(QString key)
{
    return m_settings->value(key);
}

QVariant IniEditor::read(QString key, QVariant defauld)
{
    return m_settings->value(key,defauld);
}

void IniEditor::sync()
{
    m_settings->sync();
}
