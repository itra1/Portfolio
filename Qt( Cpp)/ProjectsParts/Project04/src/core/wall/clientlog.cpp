#include "clientlog.h"
#include "../../config/config.h"
#include "../../general/settingsstorage.h"
#include "windows.h"
#include <QDateTime>
#include <QString>

ClientLog::ClientLog()
{

}

void ClientLog::init()
{
  if(!General::SettingsStorage::instance()->hashValue(saveLogKey))
    return;
	saveLog(false);
}

void ClientLog::saveLog(bool isCrash)
{
  if(General::SettingsStorage::instance()->hashValue(saveLogKey)){
      General::SettingsStorage::instance()->remove(saveLogKey);
      General::SettingsStorage::instance()->save();
    }

  QString fileSource{Config::videoWallLogFolder() + "/Player.log"};
	QString date{QDateTime::currentDateTime().toString("dd.MM.yyyy_hh.mm.ss")};
	QString targetLog{Config::currentPath() + "/wallLog"};
	QString crashName{isCrash ? "_crash" : ""};
	QString fileName{"/" + date + crashName + ".log"};

	// В ситуацйии с крашем 5ти секундное задержка, ожидаем дозапись лога
	if (isCrash)
		Sleep(5000);

	QDir tDir(targetLog);
	if (!tDir.exists())
		tDir.mkdir(targetLog);

	QFile::copy(fileSource, targetLog + fileName);
}

void ClientLog::saveReady()
{
  General::SettingsStorage::instance()->storeValue(saveLogKey,true);
  General::SettingsStorage::instance()->save();
}
