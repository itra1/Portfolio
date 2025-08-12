#include "preferences.h"

Preferences *Preferences::m_instance = nullptr;

Preferences::Preferences() = default;

Preferences *Preferences::instance()
{
    return m_instance;
}

int Preferences::getTrackerPort() const
{
    return 9000;
}

void Preferences::initInstance()
{
    if (!m_instance)
        m_instance = new Preferences;
}

void Preferences::freeInstance()
{
    if (m_instance) {
        delete m_instance;
        m_instance = nullptr;
    }
}


bool Preferences::recheckTorrentsOnCompletion() const
{
    return false;
}

void Preferences::recheckTorrentsOnCompletion(bool recheck)
{
}
