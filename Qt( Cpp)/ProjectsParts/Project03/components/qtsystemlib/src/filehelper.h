#ifndef FILEHELPER_H
#define FILEHELPER_H

#include <QString>
#include <Windows.h>
#include <iostream>
#include <sstream>

namespace QtSystemLib {

struct FileHelper
{
    static std::string GetFileVersion(std::string path);
};
} // namespace QtSystemLib
#endif // FILEHELPER_H
