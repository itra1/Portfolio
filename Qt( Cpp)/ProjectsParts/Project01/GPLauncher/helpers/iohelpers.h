#ifndef IOHELPERS_H
#define IOHELPERS_H

#include <QString>

class IOHelpers
{
public:
    IOHelpers();

    static QString currentPath();
    static QString macosPath;
};

#endif // IOHELPERS_H
