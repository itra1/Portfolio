/*
 * Bittorrent Client using Qt and libtorrent.
 */

#ifndef BITTORRENT_SESSION_H
#define BITTORRENT_SESSION_H

#include <vector>

#include <QtGlobal>
#include <QThread>
#include <QTimer>
#include <QDir>
#include <QSet>
#include <QElapsedTimer>
#include <QtDebug>
#include <QFile>
#include <QHash>
#include <QList>
#include <QMap>
#include <QNetworkConfigurationManager>
#include <QPointer>
#include <QSet>
#include <QStringList>
#include <QVector>
#include <QWaitCondition>


#include "base/bittorrent/magneturi.h"
#include "base/bittorrent/torrentinfo.h"

#include "base/utils/fs.h"
#include "base/utils/misc.h"
#include "base/utils/net.h"
#include "base/utils/random.h"
#include "base/utils/string.h"
#include "base/global.h"
#include "base/utils/fs.h"
#include "base/profile.h"
#include "base/tristatebool.h"
#include "base/types.h"
#include "base/torrentfilter.h"
#include "base/net/downloadhandler.h"
#include "base/net/downloadmanager.h"
#include "base/net/portforwarder.h"
#include "base/bittorrent/addtorrentparams.h"
#include "base/bittorrent/cachestatus.h"
#include "base/bittorrent/sessionstatus.h"
#include "base/bittorrent/torrenthandle.h"
#include "base/torrentfileguard.h"
#include "base/bittorrent/private/bandwidthscheduler.h"
#include "base/bittorrent/private/filterparserthread.h"
#include "base/bittorrent/private/resumedatasavingmanager.h"
#include "base/bittorrent/private/statistics.h"
#include "base/bittorrent/tracker.h"
#include "base/bittorrent/trackerentry.h"

#include <libtorrent/session.hpp>
#include <libtorrent/alert_types.hpp>
#include <libtorrent/session_stats.hpp>
#include <libtorrent/session_status.hpp>
#include <libtorrent/bdecode.hpp>
#include <libtorrent/bencode.hpp>
#include <libtorrent/disk_io_thread.hpp>
#include <libtorrent/error_code.hpp>
#include <libtorrent/extensions/ut_metadata.hpp>
#include <libtorrent/extensions/ut_pex.hpp>
#include <libtorrent/extensions/smart_ban.hpp>
#include <libtorrent/identify_client.hpp>
#include <libtorrent/ip_filter.hpp>
#include <libtorrent/magnet_uri.hpp>
#include <libtorrent/torrent_info.hpp>

#include "config.h"

namespace libtorrent
{
    class session;
    class entry;
    struct ip_filter;
    struct settings_pack;
    struct torrent_handle;
    class TorrentFilter;
    class Tracker;

    class alert;
    struct torrent_alert;
    struct state_update_alert;
    struct stats_alert;
    struct add_torrent_alert;
    struct torrent_checked_alert;
    struct torrent_finished_alert;
    struct torrent_removed_alert;
    struct torrent_deleted_alert;
    struct torrent_delete_failed_alert;
    struct torrent_paused_alert;
    struct torrent_resumed_alert;
    struct save_resume_data_alert;
    struct save_resume_data_failed_alert;
    struct file_renamed_alert;
    struct storage_moved_alert;
    struct storage_moved_failed_alert;
    struct metadata_received_alert;
    struct file_error_alert;
    struct file_completed_alert;
    struct tracker_error_alert;
    struct tracker_reply_alert;
    struct tracker_warning_alert;
    struct portmap_error_alert;
    struct portmap_alert;
    struct peer_blocked_alert;
    struct peer_ban_alert;
    struct fastresume_rejected_alert;
    struct url_seed_alert;
    struct listen_succeeded_alert;
    struct listen_failed_alert;
    struct external_ip_alert;
    struct session_stats_alert;
}

class QThread;
class QTimer;
class QStringList;
class QString;
class QUrl;

class FilterParserThread;
class BandwidthScheduler;
class Statistics;
class ResumeDataSavingManager;

enum MaxRatioAction
{
    Pause,
    Remove
};

enum TorrentExportFolder
{
    Regular,
    Finished
};

namespace BitTorrent
{

    class TorrentHandle;
    class InfoHash;
    class Tracker;
    class MagnetUri;
    class TrackerEntry;
    class TorrentInfo;
    struct CreateTorrentParams;
    struct AddTorrentParams;


    struct TorrentStatusReport
    {
        uint nbDownloading = 0;
        uint nbSeeding = 0;
        uint nbCompleted = 0;
        uint nbActive = 0;
        uint nbInactive = 0;
        uint nbPaused = 0;
        uint nbResumed = 0;
        uint nbErrored = 0;
    };

    class SessionSettingsEnums
    {
        Q_GADGET

    public:
        // TODO: remove `SessionSettingsEnums` wrapper when we can use `Q_ENUM_NS` directly (QT >= 5.8 only)
        enum class ChokingAlgorithm : int
        {
            FixedSlots = 0,
            RateBased = 1
        };
        Q_ENUM(ChokingAlgorithm)

        enum class SeedChokingAlgorithm : int
        {
            RoundRobin = 0,
            FastestUpload = 1,
            AntiLeech = 2
        };
        Q_ENUM(SeedChokingAlgorithm)

        enum class MixedModeAlgorithm : int
        {
            TCP = 0,
            Proportional = 1
        };
        Q_ENUM(MixedModeAlgorithm)

        enum class BTProtocol : int
        {
            Both = 0,
            TCP = 1,
            UTP = 2
        };
        Q_ENUM(BTProtocol)
    };
    using ChokingAlgorithm = SessionSettingsEnums::ChokingAlgorithm;
    using SeedChokingAlgorithm = SessionSettingsEnums::SeedChokingAlgorithm;
    using MixedModeAlgorithm = SessionSettingsEnums::MixedModeAlgorithm;
    using BTProtocol = SessionSettingsEnums::BTProtocol;

    struct SessionMetricIndices
    {
        struct
        {
            int hasIncomingConnections = 0;
            int sentPayloadBytes = 0;
            int recvPayloadBytes = 0;
            int sentBytes = 0;
            int recvBytes = 0;
            int sentIPOverheadBytes = 0;
            int recvIPOverheadBytes = 0;
            int sentTrackerBytes = 0;
            int recvTrackerBytes = 0;
            int recvRedundantBytes = 0;
            int recvFailedBytes = 0;
        } net;

        struct
        {
            int numPeersConnected = 0;
            int numPeersUpDisk = 0;
            int numPeersDownDisk = 0;
        } peer;

        struct
        {
            int dhtBytesIn = 0;
            int dhtBytesOut = 0;
            int dhtNodes = 0;
        } dht;

        struct
        {
            int diskBlocksInUse = 0;
            int numBlocksRead = 0;
            int numBlocksCacheHits = 0;
            int writeJobs = 0;
            int readJobs = 0;
            int hashJobs = 0;
            int queuedDiskJobs = 0;
            int diskJobTime = 0;
        } disk;
    };

    class Session : public QObject
    {
        Q_OBJECT
        Q_DISABLE_COPY(Session)

    public:
        static void initInstance();
        static void freeInstance();
        static Session *instance();

        QString defaultSavePath() const;
        void setDefaultSavePath(QString path);
        QString tempPath() const;
        void setTempPath(QString path);
        bool isTempPathEnabled() const;
        void setTempPathEnabled(bool enabled);
        QString torrentTempPath(const BitTorrent::TorrentInfo &torrentInfo) const;
        void configure();

        static bool isValidCategoryName(const QString &name);
        // returns category itself and all top level categories
        static QStringList expandCategory(const QString &category);

        const QStringMap &categories() const;
        QString categorySavePath(const QString &categoryName) const;
        bool addCategory(const QString &name, const QString &savePath = "");
        bool editCategory(const QString &name, const QString &savePath);
        bool removeCategory(const QString &name);
        bool isSubcategoriesEnabled() const;
        void setSubcategoriesEnabled(bool value);

        // Torrent Management Mode subsystem (TMM)
        //
        // Each torrent can be either in Manual mode or in Automatic mode
        // In Manual Mode various torrent properties are set explicitly(eg save path)
        // In Automatic Mode various torrent properties are set implicitly(eg save path)
        //     based on the associated category.
        // In Automatic Mode torrent save path can be changed in following cases:
        //     1. Default save path changed
        //     2. Torrent category save path changed
        //     3. Torrent category changed
        //     (unless otherwise is specified)
        bool isAutoTMMDisabledByDefault() const;
        void setAutoTMMDisabledByDefault(bool value);
        bool isDisableAutoTMMWhenCategoryChanged() const;
        void setDisableAutoTMMWhenCategoryChanged(bool value);
        bool isDisableAutoTMMWhenDefaultSavePathChanged() const;
        void setDisableAutoTMMWhenDefaultSavePathChanged(bool value);
        bool isDisableAutoTMMWhenCategorySavePathChanged() const;
        void setDisableAutoTMMWhenCategorySavePathChanged(bool value);

        qreal globalMaxRatio() const;
        void setGlobalMaxRatio(qreal ratio);
        int globalMaxSeedingMinutes() const;
        void setGlobalMaxSeedingMinutes(int minutes);
        bool isDHTEnabled() const;
        void setDHTEnabled(bool enabled);
        bool isLSDEnabled() const;
        void setLSDEnabled(bool enabled);
        bool isPeXEnabled() const;
        void setPeXEnabled(bool enabled);
        bool isAddTorrentPaused() const;
        void setAddTorrentPaused(bool value);
        bool isCreateTorrentSubfolder() const;
        void setCreateTorrentSubfolder(bool value);
        bool isTrackerEnabled() const;
        void setTrackerEnabled(bool enabled);
        bool isAppendExtensionEnabled() const;
        void setAppendExtensionEnabled(bool enabled);
        uint refreshInterval() const;
        void setRefreshInterval(uint value);
        bool isPreallocationEnabled() const;
        void setPreallocationEnabled(bool enabled);
        QString torrentExportDirectory() const;
        void setTorrentExportDirectory(QString path);
        QString finishedTorrentExportDirectory() const;
        void setFinishedTorrentExportDirectory(QString path);

        int globalDownloadSpeedLimit() const;
        void setGlobalDownloadSpeedLimit(int limit);
        int globalUploadSpeedLimit() const;
        void setGlobalUploadSpeedLimit(int limit);
        int altGlobalDownloadSpeedLimit() const;
        void setAltGlobalDownloadSpeedLimit(int limit);
        int altGlobalUploadSpeedLimit() const;
        void setAltGlobalUploadSpeedLimit(int limit);
        int downloadSpeedLimit() const;
        void setDownloadSpeedLimit(int limit);
        int uploadSpeedLimit() const;
        void setUploadSpeedLimit(int limit);
        bool isAltGlobalSpeedLimitEnabled() const;
        void setAltGlobalSpeedLimitEnabled(bool enabled);
        bool isBandwidthSchedulerEnabled() const;
        void setBandwidthSchedulerEnabled(bool enabled);

        uint saveResumeDataInterval() const;
        void setSaveResumeDataInterval(uint value);
        int port() const;
        void setPort(int port);
        bool useRandomPort() const;
        void setUseRandomPort(bool value);
        QString networkInterface() const;
        void setNetworkInterface(const QString &interface);
        QString networkInterfaceName() const;
        void setNetworkInterfaceName(const QString &name);
        QString networkInterfaceAddress() const;
        void setNetworkInterfaceAddress(const QString &address);
        bool isIPv6Enabled() const;
        void setIPv6Enabled(bool enabled);
        int encryption() const;
        void setEncryption(int state);
        bool isForceProxyEnabled() const;
        void setForceProxyEnabled(bool enabled);
        bool isProxyPeerConnectionsEnabled() const;
        void setProxyPeerConnectionsEnabled(bool enabled);
        ChokingAlgorithm chokingAlgorithm() const;
        void setChokingAlgorithm(ChokingAlgorithm mode);
        SeedChokingAlgorithm seedChokingAlgorithm() const;
        void setSeedChokingAlgorithm(SeedChokingAlgorithm mode);
        bool isAddTrackersEnabled() const;
        void setAddTrackersEnabled(bool enabled);
        QString additionalTrackers() const;
        void setAdditionalTrackers(const QString &trackers);
        bool isIPFilteringEnabled() const;
        void setIPFilteringEnabled(bool enabled);
        QString IPFilterFile() const;
        void setIPFilterFile(QString path);
        bool announceToAllTrackers() const;
        void setAnnounceToAllTrackers(bool val);
        bool announceToAllTiers() const;
        void setAnnounceToAllTiers(bool val);
        int asyncIOThreads() const;
        void setAsyncIOThreads(int num);
        int checkingMemUsage() const;
        void setCheckingMemUsage(int size);
        int diskCacheSize() const;
        void setDiskCacheSize(int size);
        int diskCacheTTL() const;
        void setDiskCacheTTL(int ttl);
        bool useOSCache() const;
        void setUseOSCache(bool use);
        bool isGuidedReadCacheEnabled() const;
        void setGuidedReadCacheEnabled(bool enabled);
        bool isCoalesceReadWriteEnabled() const;
        void setCoalesceReadWriteEnabled(bool enabled);
        bool isSuggestModeEnabled() const;
        void setSuggestMode(bool mode);
        int sendBufferWatermark() const;
        void setSendBufferWatermark(int value);
        int sendBufferLowWatermark() const;
        void setSendBufferLowWatermark(int value);
        int sendBufferWatermarkFactor() const;
        void setSendBufferWatermarkFactor(int value);
        bool isAnonymousModeEnabled() const;
        void setAnonymousModeEnabled(bool enabled);
        bool isQueueingSystemEnabled() const;
        void setQueueingSystemEnabled(bool enabled);
        bool ignoreSlowTorrentsForQueueing() const;
        void setIgnoreSlowTorrentsForQueueing(bool ignore);
        int downloadRateForSlowTorrents() const;
        void setDownloadRateForSlowTorrents(int rateInKibiBytes);
        int uploadRateForSlowTorrents() const;
        void setUploadRateForSlowTorrents(int rateInKibiBytes);
        int slowTorrentsInactivityTimer() const;
        void setSlowTorrentsInactivityTimer(int timeInSeconds);
        int outgoingPortsMin() const;
        void setOutgoingPortsMin(int min);
        int outgoingPortsMax() const;
        void setOutgoingPortsMax(int max);
        bool ignoreLimitsOnLAN() const;
        void setIgnoreLimitsOnLAN(bool ignore);
        bool includeOverheadInLimits() const;
        void setIncludeOverheadInLimits(bool include);
        QString announceIP() const;
        void setAnnounceIP(const QString &ip);
        bool isSuperSeedingEnabled() const;
        void setSuperSeedingEnabled(bool enabled);
        int maxConnections() const;
        void setMaxConnections(int max);
        int maxHalfOpenConnections() const;
        void setMaxHalfOpenConnections(int max);
        int maxConnectionsPerTorrent() const;
        void setMaxConnectionsPerTorrent(int max);
        int maxUploads() const;
        void setMaxUploads(int max);
        int maxUploadsPerTorrent() const;
        void setMaxUploadsPerTorrent(int max);
        int maxActiveDownloads() const;
        void setMaxActiveDownloads(int max);
        int maxActiveUploads() const;
        void setMaxActiveUploads(int max);
        int maxActiveTorrents() const;
        void setMaxActiveTorrents(int max);
        BTProtocol btProtocol() const;
        void setBTProtocol(BTProtocol protocol);
        bool isUTPRateLimited() const;
        void setUTPRateLimited(bool limited);
        MixedModeAlgorithm utpMixedMode() const;
        void setUtpMixedMode(MixedModeAlgorithm mode);
        bool multiConnectionsPerIpEnabled() const;
        void setMultiConnectionsPerIpEnabled(bool enabled);
        bool isTrackerFilteringEnabled() const;
        void setTrackerFilteringEnabled(bool enabled);
        QStringList bannedIPs() const;
        void setBannedIPs(const QStringList &newList);

        void startUpTorrents();
        BitTorrent::TorrentHandle *findTorrent(const BitTorrent::InfoHash &hash) const;
        QHash<BitTorrent::InfoHash, BitTorrent::TorrentHandle *> torrents() const;
        TorrentStatusReport torrentStatusReport() const;
        bool hasActiveTorrents() const;
        bool hasUnfinishedTorrents() const;
        bool hasRunningSeed() const;
        const BitTorrent::SessionStatus &status() const;
        const BitTorrent::CacheStatus &cacheStatus() const;
        quint64 getAlltimeDL() const;
        quint64 getAlltimeUL() const;
        bool isListening() const;

        MaxRatioAction maxRatioAction() const;
        void setMaxRatioAction(MaxRatioAction act);

        void banIP(const QString &ip);

        bool isKnownTorrent(const BitTorrent::InfoHash &hash) const;
        bool addTorrent(const QString &source, const BitTorrent::AddTorrentParams &params = BitTorrent::AddTorrentParams());
        bool addTorrent(const BitTorrent::TorrentInfo &torrentInfo, const BitTorrent::AddTorrentParams &params = BitTorrent::AddTorrentParams());
        bool addTorrent(BitTorrent::AddTorrentParams &params, const QByteArray &data);
        bool deleteTorrent(const QString &hash, bool deleteLocalFiles = false);
        bool loadMetadata(const BitTorrent::MagnetUri &magnetUri);
        bool cancelLoadMetadata(const BitTorrent::InfoHash &hash);

        void recursiveTorrentDownload(const BitTorrent::InfoHash &hash);
        void increaseTorrentsPriority(const QStringList &hashes);
        void decreaseTorrentsPriority(const QStringList &hashes);
        void topTorrentsPriority(const QStringList &hashes);
        void bottomTorrentsPriority(const QStringList &hashes);

        // TorrentHandle interface
        void handleTorrentShareLimitChanged(BitTorrent::TorrentHandle *const torrent);
        void handleTorrentNameChanged(BitTorrent::TorrentHandle *const torrent);
        void handleTorrentSavePathChanged(BitTorrent::TorrentHandle *const torrent);
        void handleTorrentCategoryChanged(BitTorrent::TorrentHandle *const torrent, const QString &oldCategory);
        void handleTorrentTagAdded(BitTorrent::TorrentHandle *const torrent, const QString &tag);
        void handleTorrentTagRemoved(BitTorrent::TorrentHandle *const torrent, const QString &tag);
        void handleTorrentSavingModeChanged(BitTorrent::TorrentHandle *const torrent);
        void handleTorrentMetadataReceived(BitTorrent::TorrentHandle *const torrent);
        void handleTorrentPaused(BitTorrent::TorrentHandle *const torrent);
        void handleTorrentResumed(BitTorrent::TorrentHandle *const torrent);
        void handleTorrentChecked(BitTorrent::TorrentHandle *const torrent);
        void handleTorrentFinished(BitTorrent::TorrentHandle *const torrent);
        void handleTorrentTrackersAdded(BitTorrent::TorrentHandle *const torrent, const QList<BitTorrent::TrackerEntry> &newTrackers);
        void handleTorrentTrackersRemoved(BitTorrent::TorrentHandle *const torrent, const QList<BitTorrent::TrackerEntry> &deletedTrackers);
        void handleTorrentTrackersChanged(BitTorrent::TorrentHandle *const torrent);
        void handleTorrentUrlSeedsAdded(BitTorrent::TorrentHandle *const torrent, const QList<QUrl> &newUrlSeeds);
        void handleTorrentUrlSeedsRemoved(BitTorrent::TorrentHandle *const torrent, const QList<QUrl> &urlSeeds);
        void handleTorrentResumeDataReady(BitTorrent::TorrentHandle *const torrent, const libtorrent::entry &data);
        void handleTorrentResumeDataFailed(BitTorrent::TorrentHandle *const torrent);
        void handleTorrentTrackerReply(BitTorrent::TorrentHandle *const torrent, const QString &trackerUrl);
        void handleTorrentTrackerWarning(BitTorrent::TorrentHandle *const torrent, const QString &trackerUrl);
        void handleTorrentTrackerError(BitTorrent::TorrentHandle *const torrent, const QString &trackerUrl);

    signals:
        void statsUpdated();
        void torrentsUpdated();
        void addTorrentFailed(const QString &error);
        void torrentAdded(BitTorrent::TorrentHandle *const torrent);
        void torrentNew(BitTorrent::TorrentHandle *const torrent);
        void torrentAboutToBeRemoved(BitTorrent::TorrentHandle *const torrent);
        void torrentRemoved();
        void torrentPaused(BitTorrent::TorrentHandle *const torrent);
        void torrentResumed(BitTorrent::TorrentHandle *const torrent);
        void torrentFinished(BitTorrent::TorrentHandle *const torrent);
        void torrentFinishedChecking(BitTorrent::TorrentHandle *const torrent);
        void torrentSavePathChanged(BitTorrent::TorrentHandle *const torrent);
        void torrentCategoryChanged(BitTorrent::TorrentHandle *const torrent, const QString &oldCategory);
        void torrentTagAdded(BitTorrent::TorrentHandle *const torrent, const QString &tag);
        void torrentTagRemoved(BitTorrent::TorrentHandle *const torrent, const QString &tag);
        void torrentSavingModeChanged(BitTorrent::TorrentHandle *const torrent);
        void allTorrentsFinished();
        void metadataLoaded(const BitTorrent::TorrentInfo &info);
        void torrentMetadataLoaded(BitTorrent::TorrentHandle *const torrent);
        void fullDiskError(BitTorrent::TorrentHandle *const torrent, const QString &msg);
        void trackerSuccess(BitTorrent::TorrentHandle *const torrent, const QString &tracker);
        void trackerWarning(BitTorrent::TorrentHandle *const torrent, const QString &tracker);
        void trackerError(BitTorrent::TorrentHandle *const torrent, const QString &tracker);
        void trackerAuthenticationRequired(BitTorrent::TorrentHandle *const torrent);
        void recursiveTorrentDownloadPossible(BitTorrent::TorrentHandle *const torrent);
        void speedLimitModeChanged(bool alternative);
        void IPFilterParsed(bool error, int ruleCount);
        void trackersAdded(BitTorrent::TorrentHandle *const torrent, const QList<BitTorrent::TrackerEntry> &trackers);
        void trackersRemoved(BitTorrent::TorrentHandle *const torrent, const QList<BitTorrent::TrackerEntry> &trackers);
        void trackersChanged(BitTorrent::TorrentHandle *const torrent);
        void trackerlessStateChanged(BitTorrent::TorrentHandle *const torrent, bool trackerless);
        void downloadFromUrlFailed(const QString &url, const QString &reason);
        void downloadFromUrlFinished(const QString &url);
        void categoryAdded(const QString &categoryName);
        void categoryRemoved(const QString &categoryName);
        void subcategoriesSupportChanged();
        void tagAdded(const QString &tag);
        void tagRemoved(const QString &tag);

    private slots:
        void updateStatusTorrent();
        void configureDeferred();
        void readAlerts();
        void refresh();
        void processShareLimits();
        void generateResumeData(bool final = false);
        void handleIPFilterParsed(int ruleCount);
        void handleIPFilterError();
        void handleDownloadFinished(const QString &url, const QByteArray &data);
        void handleDownloadFailed(const QString &url, const QString &reason);
        void handleRedirectedToMagnet(const QString &url, const QString &magnetUri);

        // Session reconfiguration triggers
        void networkOnlineStateChanged(const bool online);
        void networkConfigurationChange(const QNetworkConfiguration&);

    private:
        struct RemovingTorrentData
        {
            QString name;
            QString savePathToRemove;
            bool requestedFileDeletion;
        };

        explicit Session(QObject *parent = nullptr);
        ~Session();

        bool hasPerTorrentRatioLimit() const;
        bool hasPerTorrentSeedingTimeLimit() const;

        void initResumeFolder();

        // Session configuration
        void configure(libtorrent::settings_pack &settingsPack);
        void configurePeerClasses();
        void adjustLimits(libtorrent::settings_pack &settingsPack);
        void applyBandwidthLimits(libtorrent::settings_pack &settingsPack);
        void initMetrics();
        void adjustLimits();
        void applyBandwidthLimits();
        void processBannedIPs(libtorrent::ip_filter &filter);
        const QStringList getListeningIPs();
        void configureListeningInterface();
        void enableTracker(bool enable);
        void enableBandwidthScheduler();
        void populateAdditionalTrackers();
        void enableIPFilter();
        void disableIPFilter();

        bool addTorrent_impl(BitTorrent::CreateTorrentParams params, const BitTorrent::MagnetUri &magnetUri,
                             BitTorrent::TorrentInfo torrentInfo = BitTorrent::TorrentInfo(),
                             const QByteArray &fastresumeData = QByteArray());
        bool findIncompleteFiles(BitTorrent::TorrentInfo &torrentInfo, QString &savePath) const;

        void updateSeedingLimitTimer();
        void exportTorrentFile(BitTorrent::TorrentHandle *const torrent, TorrentExportFolder folder = TorrentExportFolder::Regular);
        void saveTorrentResumeData(BitTorrent::TorrentHandle *const torrent);

        void handleAlert(libtorrent::alert *a);
        void dispatchTorrentAlert(libtorrent::alert *a);
        void handleAddTorrentAlert(libtorrent::add_torrent_alert *p);
        void handleStateUpdateAlert(libtorrent::state_update_alert *p);
        void handleMetadataReceivedAlert(libtorrent::metadata_received_alert *p);
        void handleFileErrorAlert(libtorrent::file_error_alert *p);
        void handleTorrentRemovedAlert(libtorrent::torrent_removed_alert *p);
        void handleTorrentDeletedAlert(libtorrent::torrent_deleted_alert *p);
        void handleTorrentDeleteFailedAlert(libtorrent::torrent_delete_failed_alert *p);
        void handlePortmapWarningAlert(libtorrent::portmap_error_alert *p);
        void handlePortmapAlert(libtorrent::portmap_alert *p);
        void handlePeerBlockedAlert(libtorrent::peer_blocked_alert *p);
        void handlePeerBanAlert(libtorrent::peer_ban_alert *p);
        void handleUrlSeedAlert(libtorrent::url_seed_alert *p);
        void handleListenSucceededAlert(libtorrent::listen_succeeded_alert *p);
        void handleListenFailedAlert(libtorrent::listen_failed_alert *p);
        void handleExternalIPAlert(libtorrent::external_ip_alert *p);
        void handleSessionStatsAlert(libtorrent::session_stats_alert *p);

        void createTorrentHandle(const libtorrent::torrent_handle &nativeHandle);

        void saveResumeData();
        void saveTorrentsQueue();
        void removeTorrentsQueue();

        void getPendingAlerts(std::vector<libtorrent::alert *> &out, ulong time = 0);

        // BitTorrent
        libtorrent::session *m_nativeSession;

        bool m_deferredConfigureScheduled;
        bool m_IPFilteringChanged;
        bool m_listenInterfaceChanged; // optimization

        bool m_isDHTEnabled;
        bool m_isLSDEnabled;
        bool m_isPeXEnabled;
        bool m_isIPFilteringEnabled;
        bool m_isTrackerFilteringEnabled;
        QString m_IPFilterFile;
        bool m_announceToAllTrackers;
        bool m_announceToAllTiers;
        int m_asyncIOThreads;
        int m_checkingMemUsage;
        int m_diskCacheSize;
        int m_diskCacheTTL;
        bool m_useOSCache;
        bool m_guidedReadCacheEnabled;
        bool m_coalesceReadWriteEnabled;
        bool m_isSuggestMode;
        int m_sendBufferWatermark;
        int m_sendBufferLowWatermark;
        int m_sendBufferWatermarkFactor;
        bool m_isAnonymousModeEnabled;
        bool m_isQueueingEnabled;
        int m_maxActiveDownloads;
        int m_maxActiveUploads;
        int m_maxActiveTorrents;
        bool m_ignoreSlowTorrentsForQueueing;
        int m_downloadRateForSlowTorrents;
        int m_uploadRateForSlowTorrents;
        int m_slowTorrentsInactivityTimer;
        int m_outgoingPortsMin;
        int m_outgoingPortsMax;
        bool m_ignoreLimitsOnLAN;
        bool m_includeOverheadInLimits;
        QString m_announceIP;
        bool m_isSuperSeedingEnabled;
        int m_maxConnections;
        int m_maxHalfOpenConnections;
        int m_maxUploads;
        int m_maxConnectionsPerTorrent;
        int m_maxUploadsPerTorrent;
        BTProtocol m_btProtocol;
        bool m_isUTPRateLimited;
        MixedModeAlgorithm m_utpMixedMode;
        bool m_multiConnectionsPerIpEnabled;
        bool m_isAddTrackersEnabled;
        QString m_additionalTrackers;
        qreal m_globalMaxRatio;
        int m_globalMaxSeedingMinutes;
        bool m_isAddTorrentPaused;
        bool m_isCreateTorrentSubfolder;
        bool m_isAppendExtensionEnabled;
        uint m_refreshInterval;
        bool m_isPreallocationEnabled;
        QString m_torrentExportDirectory;
        QString m_finishedTorrentExportDirectory;
        int m_globalDownloadSpeedLimit;
        int m_globalUploadSpeedLimit;
        int m_altGlobalDownloadSpeedLimit;
        int m_altGlobalUploadSpeedLimit;
        bool m_isAltGlobalSpeedLimitEnabled;
        bool m_isBandwidthSchedulerEnabled;
        uint m_saveResumeDataInterval;
        int m_port;
        bool m_useRandomPort;
        QString m_networkInterface;
        QString m_networkInterfaceName;
        QString m_networkInterfaceAddress;
        bool m_isIPv6Enabled;
        int m_encryption;
        bool m_isForceProxyEnabled;
        bool m_isProxyPeerConnectionsEnabled;
        ChokingAlgorithm m_chokingAlgorithm;
        SeedChokingAlgorithm m_seedChokingAlgorithm;
        QVariantMap m_storedCategories;
        QStringList m_storedTags;
        int m_maxRatioAction;
        QString m_defaultSavePath;
        QString m_tempPath;
        bool m_isSubcategoriesEnabled;
        bool m_isTempPathEnabled;
        bool m_isAutoTMMDisabledByDefault;
        bool m_isDisableAutoTMMWhenCategoryChanged;
        bool m_isDisableAutoTMMWhenDefaultSavePathChanged;
        bool m_isDisableAutoTMMWhenCategorySavePathChanged;
        bool m_isTrackerEnabled;
        QStringList m_bannedIPs;

        // Order is important. This needs to be declared after its CachedSettingsValue
        // counterpart, because it uses it for initialization in the constructor
        // initialization list.
        const bool m_wasPexEnabled;

        int m_numResumeData;
        int m_extraLimit;
        QList<BitTorrent::TrackerEntry> m_additionalTrackerList;
        QString m_resumeFolderPath;
        QFile m_resumeFolderLock;
        bool m_useProxy;

        QTimer *m_refreshTimer;
        QTimer *m_seedingLimitTimer;
        QTimer *m_resumeDataTimer;
        // IP filtering
        QPointer<FilterParserThread> m_filterParser;
        QPointer<BandwidthScheduler> m_bwScheduler;
        // Tracker
        QPointer<BitTorrent::Tracker> m_tracker;
        // fastresume data writing thread
        QThread *m_ioThread;
        ResumeDataSavingManager *m_resumeDataSavingManager;

        QHash<BitTorrent::InfoHash, BitTorrent::TorrentInfo> m_loadedMetadata;
        QHash<BitTorrent::InfoHash, BitTorrent::TorrentHandle *> m_torrents;
        QHash<BitTorrent::InfoHash, BitTorrent::CreateTorrentParams> m_addingTorrents;
        QHash<QString, BitTorrent::AddTorrentParams> m_downloadedTorrents;
        QHash<BitTorrent::InfoHash, RemovingTorrentData> m_removingTorrents;
        TorrentStatusReport m_torrentStatusReport;
        QStringMap m_categories;
        QSet<QString> m_tags;

        QTimer* m_timerUpdateTorrentStatus;

        // I/O errored torrents
        QSet<BitTorrent::InfoHash> m_recentErroredTorrents;
        QTimer *m_recentErroredTorrentsTimer;

        SessionMetricIndices m_metricIndices;
        QElapsedTimer m_statsUpdateTimer;

        BitTorrent::SessionStatus m_status;
        BitTorrent::CacheStatus m_cacheStatus;

        QNetworkConfigurationManager m_networkManager;

        static Session *m_instance;
    };
}

#endif // BITTORRENT_SESSION_H
