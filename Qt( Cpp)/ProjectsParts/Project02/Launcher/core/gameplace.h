#ifndef GAMEPLACE_H
#define GAMEPLACE_H

#include <QObject>
#include "place.h"
#include "sourceinfo.h"
#include <QProcess>

namespace BitTorrent{
    class Session;
    class TorrentInfo;
    class InfoHash;
}

namespace Core{

    class GamePlace : public Place
    {
        Q_OBJECT
    public:
        explicit GamePlace(QObject *parent = nullptr);

        bool isGame() override;
        void checkState() override;
        void checkCache();
        bool isCheckChache();
        void refindLocal();
        void play();
        bool isPlay();
        bool isPlayReady();
        void update() override;
        void install();

        void setTorrentUrl(const QString &torrentUrl);

    signals:
        void onPlay(bool isPlay);

    public slots:

    private:
        QString getSourceUrl() override;
        QProcess m_gameProcess;
        QString m_torrentUrl;

    };
}

#endif // GAMEPLACE_H
