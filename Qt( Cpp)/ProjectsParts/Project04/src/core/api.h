#ifndef API_H
#define API_H

#include <QJsonArray>
#include <QNetworkReply>
#include <QString>
#include <functional>

namespace Core {

class Api
{
	public:
	static void authorization(QString userName,
														QString password,
														const std::function<void(bool, QNetworkReply *)> &onComplete);
	//! Получаем список релизов
	static void loadReleases(const std::function<void(QJsonArray)> &onComplete,
													 const std::function<void(QString)> &onError);
	//! Скачиваем один релиз
	static void loadRelease(QString url,
													const std::function<void(bool, QNetworkReply *)> &onComplete,
													const std::function<void(qint64, qint64)> &onProgress);

	private:
};
} // namespace Core
#endif // API_H
