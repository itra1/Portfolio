#ifndef IOHELPER_H
#define IOHELPER_H

#include <QString>

using namespace std;

namespace QtSystemLib {
class IOHelper
{
public:
    static void CopyFile(QString sourceFile, QString targetFile);
    static void MoveFile(QString sourceFile, QString targetFile);
    static void CopyPath(QString sourcePath, QString targetPath);
    static QByteArray TextFileRead(QString path);

private:
    IOHelper() {}
};
} // namespace QtSystemLib
#endif // IOHELPER_H
