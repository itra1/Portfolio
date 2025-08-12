#include "servers.h"
#include "helpers/iohelpers.h"
#include "rsa/rsastaticlib.h"
#include <QDebug>
#include <QDir>
#include <QFile>
#include <QJsonDocument>

Servers *Servers::_instance = nullptr;

Servers::Servers(QObject *parent) : QObject(parent) { readUrlLinks(); }
void Servers::initInstance() {
  if (!_instance)
    _instance = new Servers;
}

void Servers::freeInstance() {
  if (_instance) {
    delete _instance;
    _instance = nullptr;
  }
}

Servers *Servers::instance() { return _instance; }

void Servers::readUrlLinks() {
  #if TARGET_OS_MAC
  QString dataFileUrl = IOHelpers::currentPath() + "/Contents/dt/gm.txt";
#else
  QString dataFileUrl = IOHelpers::currentPath() + "/gm.dll";
#endif
  // todo доделать адекватный обработчик
  if (!QFile::exists(dataFileUrl))
    return;

  QByteArray dataFile;
  QFile *file = new QFile(dataFileUrl);
  if (file->open(QFile::ReadOnly)) {
    dataFile = file->readAll();
    file->close();
  }

  RSAStaticLib *lib = new RSAStaticLib();
  std::string dataFromFile = lib->decruptFromBase64(dataFile);
  qDebug() << QByteArray::fromStdString(dataFromFile);
  parseServersData(QByteArray::fromStdString(dataFromFile));
  EmitSetServers();
}

void Servers::parseServersData(QByteArray data) {
  QJsonDocument document = QJsonDocument::fromJson(data);
  QJsonObject jObj = document.object();
  servers = ServersData::Parce(jObj, data);
}

QByteArray Servers::getServersDataBase64() {
  return servers->sources.toBase64();
}

QString Servers::server() { return servers->Apps[0]; }

QString Servers::serverApi() { return server() + "/api/v1"; }

bool Servers::serversLoaded() { return servers != nullptr; }

void Servers::EmitSetServers() { emit OnSetServer(); }
