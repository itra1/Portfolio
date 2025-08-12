#ifndef RELEASEPHASE_H
#define RELEASEPHASE_H

#include <QObject>

namespace Core
{

	class ReleaseState : public QObject {
		Q_OBJECT

		public:
		enum State {
			None = 0x0,
			Loading = 0x1,
			Downloaded = 0x2,
			Unpack = 0x4,
			Installed = 0x8,
			Played = 0x16,
			StartPlayed = 0x32
		};
		Q_ENUM(State)
	};
} // namespace Core

#endif // RELEASEPHASE_H
