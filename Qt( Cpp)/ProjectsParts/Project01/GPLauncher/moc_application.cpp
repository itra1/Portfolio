/****************************************************************************
** Meta object code from reading C++ file 'application.h'
**
** Created by: The Qt Meta Object Compiler version 67 (Qt 5.15.2)
**
** WARNING! All changes made in this file will be lost!
*****************************************************************************/

#include <memory>
#include "core/application.h"
#include <QtCore/qbytearray.h>
#include <QtCore/qmetatype.h>
#if !defined(Q_MOC_OUTPUT_REVISION)
#error "The header file 'application.h' doesn't include <QObject>."
#elif Q_MOC_OUTPUT_REVISION != 67
#error "This file was generated using the moc from 5.15.2. It"
#error "cannot be used with the include files from this version of Qt."
#error "(The moc has changed too much.)"
#endif

QT_BEGIN_MOC_NAMESPACE
QT_WARNING_PUSH
QT_WARNING_DISABLE_DEPRECATED
struct qt_meta_stringdata_Application_t {
    QByteArrayData data[12];
    char stringdata0[137];
};
#define QT_MOC_LITERAL(idx, ofs, len) \
    Q_STATIC_BYTE_ARRAY_DATA_HEADER_INITIALIZER_WITH_OFFSET(len, \
    qptrdiff(offsetof(qt_meta_stringdata_Application_t, stringdata0) + ofs \
        - idx * sizeof(QByteArrayData)) \
    )
static const qt_meta_stringdata_Application_t qt_meta_stringdata_Application = {
    {
QT_MOC_LITERAL(0, 0, 11), // "Application"
QT_MOC_LITERAL(1, 12, 13), // "onChangePhase"
QT_MOC_LITERAL(2, 26, 0), // ""
QT_MOC_LITERAL(3, 27, 18), // "onDownloadProgress"
QT_MOC_LITERAL(4, 46, 8), // "progress"
QT_MOC_LITERAL(5, 55, 17), // "connectLostSignal"
QT_MOC_LITERAL(6, 73, 8), // "onNewLog"
QT_MOC_LITERAL(7, 82, 10), // "serversSet"
QT_MOC_LITERAL(8, 93, 4), // "quit"
QT_MOC_LITERAL(9, 98, 16), // "tryConnectAgaine"
QT_MOC_LITERAL(10, 115, 13), // "isConnectLost"
QT_MOC_LITERAL(11, 129, 7) // "logText"

    },
    "Application\0onChangePhase\0\0"
    "onDownloadProgress\0progress\0"
    "connectLostSignal\0onNewLog\0serversSet\0"
    "quit\0tryConnectAgaine\0isConnectLost\0"
    "logText"
};
#undef QT_MOC_LITERAL

static const uint qt_meta_data_Application[] = {

 // content:
       8,       // revision
       0,       // classname
       0,    0, // classinfo
       7,   14, // methods
       2,   58, // properties
       0,    0, // enums/sets
       0,    0, // constructors
       0,       // flags
       4,       // signalCount

 // signals: name, argc, parameters, tag, flags
       1,    0,   49,    2, 0x06 /* Public */,
       3,    1,   50,    2, 0x06 /* Public */,
       5,    0,   53,    2, 0x06 /* Public */,
       6,    0,   54,    2, 0x06 /* Public */,

 // slots: name, argc, parameters, tag, flags
       7,    0,   55,    2, 0x0a /* Public */,

 // methods: name, argc, parameters, tag, flags
       8,    0,   56,    2, 0x02 /* Public */,
       9,    0,   57,    2, 0x02 /* Public */,

 // signals: parameters
    QMetaType::Void,
    QMetaType::Void, QMetaType::Double,    4,
    QMetaType::Void,
    QMetaType::Void,

 // slots: parameters
    QMetaType::Void,

 // methods: parameters
    QMetaType::Void,
    QMetaType::Void,

 // properties: name, type, flags
      10, QMetaType::Bool, 0x00495001,
      11, QMetaType::QString, 0x00495001,

 // properties: notify_signal_id
       2,
       3,

       0        // eod
};

void Application::qt_static_metacall(QObject *_o, QMetaObject::Call _c, int _id, void **_a)
{
    if (_c == QMetaObject::InvokeMetaMethod) {
        auto *_t = static_cast<Application *>(_o);
        Q_UNUSED(_t)
        switch (_id) {
        case 0: _t->onChangePhase(); break;
        case 1: _t->onDownloadProgress((*reinterpret_cast< double(*)>(_a[1]))); break;
        case 2: _t->connectLostSignal(); break;
        case 3: _t->onNewLog(); break;
        case 4: _t->serversSet(); break;
        case 5: _t->quit(); break;
        case 6: _t->tryConnectAgaine(); break;
        default: ;
        }
    } else if (_c == QMetaObject::IndexOfMethod) {
        int *result = reinterpret_cast<int *>(_a[0]);
        {
            using _t = void (Application::*)();
            if (*reinterpret_cast<_t *>(_a[1]) == static_cast<_t>(&Application::onChangePhase)) {
                *result = 0;
                return;
            }
        }
        {
            using _t = void (Application::*)(double );
            if (*reinterpret_cast<_t *>(_a[1]) == static_cast<_t>(&Application::onDownloadProgress)) {
                *result = 1;
                return;
            }
        }
        {
            using _t = void (Application::*)();
            if (*reinterpret_cast<_t *>(_a[1]) == static_cast<_t>(&Application::connectLostSignal)) {
                *result = 2;
                return;
            }
        }
        {
            using _t = void (Application::*)();
            if (*reinterpret_cast<_t *>(_a[1]) == static_cast<_t>(&Application::onNewLog)) {
                *result = 3;
                return;
            }
        }
    }
#ifndef QT_NO_PROPERTIES
    else if (_c == QMetaObject::ReadProperty) {
        auto *_t = static_cast<Application *>(_o);
        Q_UNUSED(_t)
        void *_v = _a[0];
        switch (_id) {
        case 0: *reinterpret_cast< bool*>(_v) = _t->getIsConnectLost(); break;
        case 1: *reinterpret_cast< QString*>(_v) = _t->getLogText(); break;
        default: break;
        }
    } else if (_c == QMetaObject::WriteProperty) {
    } else if (_c == QMetaObject::ResetProperty) {
    }
#endif // QT_NO_PROPERTIES
}

QT_INIT_METAOBJECT const QMetaObject Application::staticMetaObject = { {
    QMetaObject::SuperData::link<QObject::staticMetaObject>(),
    qt_meta_stringdata_Application.data,
    qt_meta_data_Application,
    qt_static_metacall,
    nullptr,
    nullptr
} };


const QMetaObject *Application::metaObject() const
{
    return QObject::d_ptr->metaObject ? QObject::d_ptr->dynamicMetaObject() : &staticMetaObject;
}

void *Application::qt_metacast(const char *_clname)
{
    if (!_clname) return nullptr;
    if (!strcmp(_clname, qt_meta_stringdata_Application.stringdata0))
        return static_cast<void*>(this);
    return QObject::qt_metacast(_clname);
}

int Application::qt_metacall(QMetaObject::Call _c, int _id, void **_a)
{
    _id = QObject::qt_metacall(_c, _id, _a);
    if (_id < 0)
        return _id;
    if (_c == QMetaObject::InvokeMetaMethod) {
        if (_id < 7)
            qt_static_metacall(this, _c, _id, _a);
        _id -= 7;
    } else if (_c == QMetaObject::RegisterMethodArgumentMetaType) {
        if (_id < 7)
            *reinterpret_cast<int*>(_a[0]) = -1;
        _id -= 7;
    }
#ifndef QT_NO_PROPERTIES
    else if (_c == QMetaObject::ReadProperty || _c == QMetaObject::WriteProperty
            || _c == QMetaObject::ResetProperty || _c == QMetaObject::RegisterPropertyMetaType) {
        qt_static_metacall(this, _c, _id, _a);
        _id -= 2;
    } else if (_c == QMetaObject::QueryPropertyDesignable) {
        _id -= 2;
    } else if (_c == QMetaObject::QueryPropertyScriptable) {
        _id -= 2;
    } else if (_c == QMetaObject::QueryPropertyStored) {
        _id -= 2;
    } else if (_c == QMetaObject::QueryPropertyEditable) {
        _id -= 2;
    } else if (_c == QMetaObject::QueryPropertyUser) {
        _id -= 2;
    }
#endif // QT_NO_PROPERTIES
    return _id;
}

// SIGNAL 0
void Application::onChangePhase()
{
    QMetaObject::activate(this, &staticMetaObject, 0, nullptr);
}

// SIGNAL 1
void Application::onDownloadProgress(double _t1)
{
    void *_a[] = { nullptr, const_cast<void*>(reinterpret_cast<const void*>(std::addressof(_t1))) };
    QMetaObject::activate(this, &staticMetaObject, 1, _a);
}

// SIGNAL 2
void Application::connectLostSignal()
{
    QMetaObject::activate(this, &staticMetaObject, 2, nullptr);
}

// SIGNAL 3
void Application::onNewLog()
{
    QMetaObject::activate(this, &staticMetaObject, 3, nullptr);
}
QT_WARNING_POP
QT_END_MOC_NAMESPACE
