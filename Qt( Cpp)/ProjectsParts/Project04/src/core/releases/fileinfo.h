#ifndef FILEINFO_H
#define FILEINFO_H
#include <QString>
namespace Core
{
	struct FileInfo
	{
		QString fieldname;
		QString originalname;
		QString encoding;
		QString mimetype;
		QString destination;
		QString filename;
		QString path;
		qint64 size;
		QString url;
		QString src;
		QString title;
	};
} // namespace Core
#endif // FILEINFO_H
