#ifndef FILEINFO_H
#define FILEINFO_H

#include <QString>
#include <Windows.h>
#include <iostream>
#include <sstream>

namespace Helpers {

class FileHelper
{
    static std::string GetFileVersion(QString path);
};
}  // namespace Helpers
#endif // FILEINFO_H
