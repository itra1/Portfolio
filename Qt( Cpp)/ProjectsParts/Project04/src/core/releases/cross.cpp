#include "cross.h"
#include <QDir>
#include "../../config/config.h"
#include "src/iohelper.h"

namespace Core {
Cross::Cross()
		: RunRelease()
{}

Cross::Cross(QJsonObject jObject)
		: RunRelease(jObject)
{}

void Cross::afterUnpack()
{
	QDir installfolder(installPath());
	QStringList filesList = installfolder.entryList(QDir::Files);

	for (int i = 0; i < filesList.count(); i++) {
		QString val = filesList.value(i);
		QString resPath(installPath() + "/" + val);
		auto slit = val.split(".");
		qDebug() << slit.length();
		qDebug() << resPath;
		if (slit[slit.length() - 1] == "exe")
            QtSystemLib::IOHelper::MoveFile(installPath() + "/" + val,
                                            installPath() + "/"
                                                + Config::getStringValue(ConfigKeys::crossExe));
    }
}

const QString Cross::exePath() const
{
	return installPath() + "/" + Config::getStringValue(ConfigKeys::crossExe);
}

//const bool Cross::existsDisk() {}
} // namespace Core
