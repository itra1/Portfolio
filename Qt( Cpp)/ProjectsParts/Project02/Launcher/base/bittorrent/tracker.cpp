/*
 * Bittorrent Client using Qt and libtorrent.
 */

#include "tracker.h"

#include <vector>

#include <libtorrent/bencode.hpp>
#include <libtorrent/entry.hpp>

#include "base/global.h"
#include "base/http/server.h"
#include "base/preferences.h"
#include "base/utils/bytearray.h"
#include "base/utils/string.h"

// static limits
static const int MAX_TORRENTS = 100;
static const int MAX_PEERS_PER_TORRENT = 1000;
static const int ANNOUNCE_INTERVAL = 1800; // 30min

using namespace BitTorrent;

// Peer
bool Peer::operator!=(const Peer &other) const
{
    return uid() != other.uid();
}

bool Peer::operator==(const Peer &other) const
{
    return uid() == other.uid();
}

QString Peer::uid() const
{
    return ip.toString() + ':' + QString::number(port);
}

libtorrent::entry Peer::toEntry(bool noPeerId) const
{
    libtorrent::entry::dictionary_type peerMap;
    if (!noPeerId)
        peerMap["id"] = libtorrent::entry(peerId.toStdString());
    peerMap["ip"] = libtorrent::entry(ip.toString().toStdString());
    peerMap["port"] = libtorrent::entry(port);

    return libtorrent::entry(peerMap);
}

// Tracker

Tracker::Tracker(QObject *parent)
    : QObject(parent)
    , m_server(new Http::Server(this, this))
{
}

Tracker::~Tracker()
{
    if (m_server->isListening())
        qDebug("Shutting down the embedded tracker...");
    // TODO: Store the torrent list
}

bool Tracker::start()
{
    const int listenPort = Preferences::instance()->getTrackerPort();

    if (m_server->isListening()) {
        if (m_server->serverPort() == listenPort) {
            // Already listening on the right port, just return
            return true;
        }
        // Wrong port, closing the server
        m_server->close();
    }

    qDebug("Starting the embedded tracker...");
    // Listen on the predefined port
    return m_server->listen(QHostAddress::Any, listenPort);
}

Http::Response Tracker::processRequest(const Http::Request &request, const Http::Environment &env)
{
    clear(); // clear response

    //qDebug("Tracker received the following request:\n%s", qUtf8Printable(parser.toString()));
    // Is request a GET request?
    if (request.method != "GET") {
        qDebug("Tracker: Unsupported HTTP request: %s", qUtf8Printable(request.method));
        status(100, "Invalid request type");
    }
    else if (!request.path.startsWith("/announce", Qt::CaseInsensitive)) {
        qDebug("Tracker: Unrecognized path: %s", qUtf8Printable(request.path));
        status(100, "Invalid request type");
    }
    else {
        // OK, this is a GET request
        m_request = request;
        m_env = env;
        respondToAnnounceRequest();
    }

    return response();
}

void Tracker::respondToAnnounceRequest()
{
    QMap<QString, QByteArray> queryParams;
    // Parse GET parameters
    using namespace Utils::ByteArray;
    for (const QByteArray &param : asConst(splitToViews(m_request.query, "&"))) {
        const int sepPos = param.indexOf('=');
        if (sepPos <= 0) continue; // ignores params without name

        const QByteArray nameComponent = midView(param, 0, sepPos);
        const QByteArray valueComponent = midView(param, (sepPos + 1));

        const QString paramName = QString::fromUtf8(QByteArray::fromPercentEncoding(nameComponent));
        const QByteArray paramValue = QByteArray::fromPercentEncoding(valueComponent);
        queryParams[paramName] = paramValue;
    }

    TrackerAnnounceRequest announceReq;

    // IP
    // Use the "ip" parameter provided from tracker request first, then fall back to client IP if invalid
    const QHostAddress paramIP {QString::fromLatin1(queryParams.value("ip"))};
    announceReq.peer.ip = paramIP.isNull() ? m_env.clientAddress : paramIP;

    // 1. Get info_hash
    if (!queryParams.contains("info_hash")) {
        qDebug("Tracker: Missing info_hash");
        status(101, "Missing info_hash");
        return;
    }
    announceReq.infoHash = queryParams.value("info_hash");
    // info_hash cannot be longer than 20 bytes
    /*if (annonce_req.info_hash.toLatin1().length() > 20) {
        qDebug("Tracker: Info_hash is not 20 byte long: %s (%d)", qUtf8Printable(annonce_req.info_hash), annonce_req.info_hash.toLatin1().length());
        status(150, "Invalid infohash");
        return;
      }*/

    // 2. Get peer ID
    if (!queryParams.contains("peer_id")) {
        qDebug("Tracker: Missing peer_id");
        status(102, "Missing peer_id");
        return;
    }
    announceReq.peer.peerId = queryParams.value("peer_id");
    // peer_id cannot be longer than 20 bytes
    /*if (annonce_req.peer.peer_id.length() > 20) {
        qDebug("Tracker: peer_id is not 20 byte long: %s", qUtf8Printable(annonce_req.peer.peer_id));
        status(151, "Invalid peerid");
        return;
      }*/

    // 3. Get port
    if (!queryParams.contains("port")) {
        qDebug("Tracker: Missing port");
        status(103, "Missing port");
        return;
    }
    bool ok = false;
    announceReq.peer.port = queryParams.value("port").toInt(&ok);
    if (!ok || (announceReq.peer.port < 0) || (announceReq.peer.port > 65535)) {
        qDebug("Tracker: Invalid port number (%d)", announceReq.peer.port);
        status(103, "Missing port");
        return;
    }

    // 4.  Get event
    announceReq.event = "";
    if (queryParams.contains("event")) {
        announceReq.event = queryParams.value("event");
        qDebug("Tracker: event is %s", qUtf8Printable(announceReq.event));
    }

    // 5. Get numwant
    announceReq.numwant = 50;
    if (queryParams.contains("numwant")) {
        int tmp = queryParams.value("numwant").toInt();
        if (tmp > 0) {
            qDebug("Tracker: numwant = %d", tmp);
            announceReq.numwant = tmp;
        }
    }

    // 6. no_peer_id (extension)
    announceReq.noPeerId = false;
    if (queryParams.contains("no_peer_id"))
        announceReq.noPeerId = true;

    // 7. TODO: support "compact" extension

    // Done parsing, now let's reply
    if (announceReq.event == "stopped") {
        unregisterPeer(announceReq);
    }
    else {
        registerPeer(announceReq);
        replyWithPeerList(announceReq);
    }
}

void Tracker::registerPeer(const TrackerAnnounceRequest &announceReq)
{
    if (announceReq.peer.port == 0) return;

    if (!m_torrents.contains(announceReq.infoHash)) {
        // Unknown torrent
        if (m_torrents.size() == MAX_TORRENTS) {
            // Reached max size, remove a random torrent
            m_torrents.erase(m_torrents.begin());
        }
    }

    // Register the user
    PeerList &peers = m_torrents[announceReq.infoHash];
    if (!peers.contains(announceReq.peer.uid())) {
        // Unknown peer
        if (peers.size() == MAX_PEERS_PER_TORRENT) {
            // Too many peers, remove a random one
            peers.erase(peers.begin());
        }
    }
    peers[announceReq.peer.uid()] = announceReq.peer;
}

void Tracker::unregisterPeer(const TrackerAnnounceRequest &announceReq)
{
    if (announceReq.peer.port == 0) return;

    if (m_torrents[announceReq.infoHash].remove(announceReq.peer.uid()) > 0)
        qDebug("Tracker: Peer stopped downloading, deleting it from the list");
}

void Tracker::replyWithPeerList(const TrackerAnnounceRequest &announceReq)
{
    // Prepare the entry for bencoding
    libtorrent::entry::dictionary_type replyDict;
    replyDict["interval"] = libtorrent::entry(ANNOUNCE_INTERVAL);

    libtorrent::entry::list_type peerList;
    for (const Peer &p : m_torrents.value(announceReq.infoHash))
        peerList.push_back(p.toEntry(announceReq.noPeerId));
    replyDict["peers"] = libtorrent::entry(peerList);

    const libtorrent::entry replyEntry(replyDict);
    // bencode
    QByteArray reply;
    libtorrent::bencode(std::back_inserter(reply), replyEntry);
    qDebug("Tracker: reply with the following bencoded data:\n %s", reply.constData());

    // HTTP reply
    print(reply, Http::CONTENT_TYPE_TXT);
}
