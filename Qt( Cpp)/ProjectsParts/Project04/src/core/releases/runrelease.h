#ifndef RUNRELEASE_H
#define RUNRELEASE_H

#include "release.h"

namespace Core {
class RunRelease : public Release
{
	Q_OBJECT
	public:
	RunRelease();
	RunRelease(QJsonObject jObject);
	Q_INVOKABLE void run();
	Q_INVOKABLE void install();
	bool checkState() override;
	virtual void afterUnzip();
	void unpack(Release *parentRelease = nullptr) override;
	bool isRunned() override;

	public slots:
	void onPresentationTime();
	void readOutput();
	void playComplete(int exitCode, QProcess::ExitStatus exitStatus);
	void playOutput();

	protected:
	QProcess *_playProcess = nullptr;

	private:
	QDateTime _timeRunApp;
	QTimer *_restartTimer = nullptr;
	bool _isTimerRestart = false;

    QStringList makeArgumentsDefault();
    QStringList makeArgumentsCross();
};
} // namespace Core
#endif // RUNRELEASE_H
