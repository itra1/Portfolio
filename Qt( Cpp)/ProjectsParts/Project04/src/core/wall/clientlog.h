#ifndef CLIENTLOG_H
#define CLIENTLOG_H

#include <QString>

class ClientLog
{
public:
  ClientLog();
  inline static const QString saveLogKey{"saveLog"};

  static void init();
	static void saveLog(bool isCrash);
	static void saveReady();

};

#endif // CLIENTLOG_H
