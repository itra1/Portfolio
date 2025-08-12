#ifndef IBASE_H
#define IBASE_H

//! Базовый класс для всех интерфейсов
struct IBase
{

	public:
	IBase &operator=(const IBase &) = delete;

	protected:
	virtual ~IBase() = default;
};

#endif // IBASE_H
