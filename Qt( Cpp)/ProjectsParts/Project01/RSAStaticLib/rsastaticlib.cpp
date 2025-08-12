#include "rsastaticlib.h"
#include "Qt-Secret/Qt-RSA/qrsaencryption.h"

QRSAEncryption cruptLenght = QRSAEncryption::Rsa::RSA_2048;

RSAStaticLib::RSAStaticLib()
{
}

std::string RSAStaticLib::encruptToBase64(QByteArray source)
{
  return encruptToBase64(source.toStdString());
}

std::string RSAStaticLib::encruptToBase64(std::string source)
{
  QByteArray res = encrupt(source);
  return res.toBase64().toStdString();
}

QByteArray RSAStaticLib::encrupt(QByteArray source)
{
  QByteArray pubKey = QByteArray::fromBase64(publicKey);
  QRSAEncryption e(cruptLenght);
  return e.encode(source, pubKey);
}

QByteArray RSAStaticLib::encrupt(std::string source)
{
  return encrupt(QByteArray::fromStdString(source));
}

std::string RSAStaticLib::decruptFromBase64(QByteArray source)
{
  return decrypt(QByteArray::fromBase64(source)).toStdString();
}

std::string RSAStaticLib::decruptFromBase64(std::string source)
{
  auto res = QByteArray::fromBase64(QByteArray::fromStdString(source));
  return decrypt(res).toStdString();
}

std::string RSAStaticLib::decruptFromBase64Str(std::string source)
{
  auto res = QByteArray::fromBase64(QByteArray::fromStdString(source));
  return decrypt(res).toStdString();
}

std::string RSAStaticLib::decruptFromBase64Char(char *source)
{
  auto res = QByteArray::fromBase64(QByteArray::fromStdString(source));
  return decrypt(res).toStdString();
}

QByteArray RSAStaticLib::decrypt(QByteArray source)
{
  QByteArray privKey = QByteArray::fromBase64(privateKey);
  QRSAEncryption e(cruptLenght);
  return e.decode(source, privKey);
}

QByteArray RSAStaticLib::decrypt(std::string source)
{
  return decrypt(QByteArray::fromStdString(source));
}
