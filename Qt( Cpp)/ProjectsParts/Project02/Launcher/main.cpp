#include <QApplication>
#include <QQmlApplicationEngine>
#include "QDebug"

#include "core/application.h"

#include <windows.h>
#include <GdbSrvControllerLib/stdafx.h>
#include <comdef.h>
#include <Wbemidl.h>
#include "core/logging.h"

void CheckProcess();

QString getDevice();

QString ProcessorData(HRESULT hres, IEnumWbemClassObject* pEnumerator = NULL);
QString BiosData(HRESULT hres, IEnumWbemClassObject* pEnumerator = NULL);
QString BaseBoard(HRESULT hres, IEnumWbemClassObject* pEnumerator = NULL);

int main(int argc, char *argv[])
{

    QString data = getDevice();

    QStringList paths = QCoreApplication::libraryPaths();

    paths.append(".");
    paths.append("libraries");
    paths.append("plugin");
    paths.append("bin");
    paths.append("bin/imageformats");
    paths.append("bin/platforms");

    paths.append("bin/bearer");
    paths.append("bin/resources");
    paths.append("bin/scenegraph");
    paths.append("bin/styles");
    paths.append("bin/translations");

    QCoreApplication::setLibraryPaths(paths);

    QCoreApplication::setAttribute(Qt::AA_EnableHighDpiScaling);

    Application application;


    return application.Initiate(argc, argv, data);
}

QString getDevice()
{
    SYSTEM_INFO siSysInfo;

    GetSystemInfo(&siSysInfo);

    MEMORYSTATUSEX siMemory;

    GlobalMemoryStatusEx(&siMemory);

    HRESULT hres;
    // Step 1: --------------------------------------------------
    // Initialize COM. ------------------------------------------

    hres =  CoInitializeEx(0, COINIT_MULTITHREADED);
    if (FAILED(hres))
    {
        qDebug() << "Failed to initialize COM library. Error code = 0x"
            << hex << hres << endl;
        return "";                  // Program has failed.
    }

    // Step 2: --------------------------------------------------
    // Set general COM security levels --------------------------
    // Note: If you are using Windows 2000, you need to specify -
    // the default authentication credentials for a user by using
    // a SOLE_AUTHENTICATION_LIST structure in the pAuthList ----
    // parameter of CoInitializeSecurity ------------------------

    hres =  CoInitializeSecurity(
        NULL,
        -1,                          // COM authentication
        NULL,                        // Authentication services
        NULL,                        // Reserved
        RPC_C_AUTHN_LEVEL_DEFAULT,   // Default authentication
        RPC_C_IMP_LEVEL_IMPERSONATE, // Default Impersonation
        NULL,                        // Authentication info
        EOAC_NONE,                   // Additional capabilities
        NULL                         // Reserved
        );


    if (FAILED(hres))
    {
        qDebug() << "Failed to initialize security. Error code = 0x"
            << hex << hres << endl;
        CoUninitialize();
        return "";                    // Program has failed.
    }

    // Step 3: ---------------------------------------------------
    // Obtain the initial locator to WMI -------------------------

    IWbemLocator *pLoc = NULL;

    hres = CoCreateInstance(
        CLSID_WbemLocator,
        0,
        CLSCTX_INPROC_SERVER,
        IID_IWbemLocator, (LPVOID *) &pLoc);

    if (FAILED(hres))
    {
        qDebug() << "Failed to create IWbemLocator object."
            << " Err code = 0x"
            << hex << hres << endl;
        CoUninitialize();
        return "";                 // Program has failed.
    }

    // Step 4: -----------------------------------------------------
    // Connect to WMI through the IWbemLocator::ConnectServer method

    IWbemServices *pSvc = NULL;

    // Connect to the root\cimv2 namespace with
    // the current user and obtain pointer pSvc
    // to make IWbemServices calls.
    hres = pLoc->ConnectServer(
         _bstr_t(L"ROOT\\CIMV2"), // Object path of WMI namespace
         NULL,                    // User name. NULL = current user
         NULL,                    // User password. NULL = current
         0,                       // Locale. NULL indicates current
         NULL,                    // Security flags.
         0,                       // Authority (e.g. Kerberos)
         0,                       // Context object
         &pSvc                    // pointer to IWbemServices proxy
         );

    if (FAILED(hres))
    {
        qDebug() << "Could not connect. Error code = 0x"
             << hex << hres << endl;
        pLoc->Release();
        CoUninitialize();
        return "";                // Program has failed.
    }

    qDebug() << "Connected to ROOT\\CIMV2 WMI namespace" << endl;

    // Step 5: --------------------------------------------------
    // Set security levels on the proxy -------------------------

    hres = CoSetProxyBlanket(
       pSvc,                        // Indicates the proxy to set
       RPC_C_AUTHN_WINNT,           // RPC_C_AUTHN_xxx
       RPC_C_AUTHZ_NONE,            // RPC_C_AUTHZ_xxx
       NULL,                        // Server principal name
       RPC_C_AUTHN_LEVEL_CALL,      // RPC_C_AUTHN_LEVEL_xxx
       RPC_C_IMP_LEVEL_IMPERSONATE, // RPC_C_IMP_LEVEL_xxx
       NULL,                        // client identity
       EOAC_NONE                    // proxy capabilities
    );

    if (FAILED(hres))
    {
        qDebug() << "Could not set proxy blanket. Error code = 0x"
            << hex << hres << endl;
        pSvc->Release();
        pLoc->Release();
        CoUninitialize();
        return "" ;               // Program has failed.
    }

    IEnumWbemClassObject* pEnumerator1 = NULL;
    HRESULT hres1 = pSvc->ExecQuery(
        bstr_t("WQL"),
        bstr_t("SELECT * FROM Win32_Processor"),
        WBEM_FLAG_FORWARD_ONLY | WBEM_FLAG_RETURN_IMMEDIATELY,
        NULL,
        &pEnumerator1);

    IEnumWbemClassObject* pEnumerator2 = NULL;
    HRESULT hres2 = pSvc->ExecQuery(
        bstr_t("WQL"),
        bstr_t("SELECT * FROM Win32_BIOS"),
        WBEM_FLAG_FORWARD_ONLY | WBEM_FLAG_RETURN_IMMEDIATELY,
        NULL,
        &pEnumerator2);

    IEnumWbemClassObject* pEnumerator3 = NULL;
    HRESULT hres3 = pSvc->ExecQuery(
        bstr_t("WQL"),
        bstr_t("SELECT * FROM Win32_BaseBoard"),
        WBEM_FLAG_FORWARD_ONLY | WBEM_FLAG_RETURN_IMMEDIATELY,
        NULL,
        &pEnumerator3);

    QString dataOut = "";
    dataOut += ProcessorData(hres1,pEnumerator1);
    dataOut += BiosData(hres2,pEnumerator2);
    dataOut += BaseBoard(hres3,pEnumerator3);

    // Cleanup
    // ========

    pSvc->Release();
    pLoc->Release();
    CoUninitialize();
    return dataOut;
}

QString ProcessorData(HRESULT hres, IEnumWbemClassObject *pEnumerator){

//    HRESULT hres;
//    // Step 6: --------------------------------------------------
//    // Use the IWbemServices pointer to make requests of WMI ----

//    // For example, get the name of the operating system
//    IEnumWbemClassObject* pEnumerator = NULL;
//    hres = pSvc->ExecQuery(
//        bstr_t("WQL"),
//        bstr_t("SELECT * FROM Win32_Processor"),
//        WBEM_FLAG_FORWARD_ONLY | WBEM_FLAG_RETURN_IMMEDIATELY,
//        NULL,
//        &pEnumerator);

//    if (FAILED(hres))
//    {
//        qDebug() << "Query for operating system name failed."
//            << " Error code = 0x"
//            << hex << hres << endl;
//        pSvc->Release();
//        CoUninitialize();
//        return "";               // Program has failed.
//    }

    QString outData = "Processor:\n";

    // Step 7: -------------------------------------------------
    // Get the data from the query in step 6 -------------------

    IWbemClassObject *pclsObj;
    ULONG uReturn = 0;

    while (pEnumerator)
    {
        HRESULT hr = pEnumerator->Next(WBEM_INFINITE, 1,
            &pclsObj, &uReturn);

        if(0 == uReturn)
        {
            break;
        }

        VARIANT UniqueID;
        VARIANT ProcessorID;
        VARIANT Name;
        VARIANT Manufacturer;
        VARIANT MaxClockSpeed;

        // Get the value of the Name property

        hr = pclsObj->Get(L"UniqueID", 0, &UniqueID , 0, 0);
        outData += QString("  UniqueID:%1\n").arg(UniqueID.bstrVal);
        hr = pclsObj->Get(L"ProcessorID", 0, &ProcessorID , 0, 0);
        outData += QString("  ProcessorID:%1\n").arg(ProcessorID.bstrVal);
        hr = pclsObj->Get(L"Name", 0, &Name , 0, 0);
        outData += QString("  Name:%1\n").arg(Name.bstrVal);
        hr = pclsObj->Get(L"Manufacturer", 0, &Manufacturer , 0, 0);
        outData += QString("  Manufacturer:%1\n").arg(Manufacturer.bstrVal);
        hr = pclsObj->Get(L"MaxClockSpeed", 0, &MaxClockSpeed , 0, 0);
        outData += QString("  MaxClockSpeed:%1\n").arg(Manufacturer.uintVal);

        VariantClear(&UniqueID);
        VariantClear(&ProcessorID);
        VariantClear(&Name);
        VariantClear(&Manufacturer);
        VariantClear(&MaxClockSpeed);
    }

    pEnumerator->Release();
    pclsObj->Release();
    return outData;
}

QString BiosData(HRESULT hres, IEnumWbemClassObject *pEnumerator){

//    HRESULT hres;
//    // Step 6: --------------------------------------------------
//    // Use the IWbemServices pointer to make requests of WMI ----

//    // For example, get the name of the operating system
//    IEnumWbemClassObject* pEnumerator = NULL;
//    hres = pSvc->ExecQuery(
//        bstr_t("WQL"),
//        bstr_t("SELECT * FROM Win32_BIOS"),
//        WBEM_FLAG_FORWARD_ONLY | WBEM_FLAG_RETURN_IMMEDIATELY,
//        NULL,
//        &pEnumerator);

//    if (FAILED(hres))
//    {
//        qDebug() << "Query for operating system name failed."
//            << " Error code = 0x"
//            << hex << hres << endl;
//        pSvc->Release();
//        CoUninitialize();
//        return "";               // Program has failed.
//    }
    QString outData = "Bios:\n";

    // Step 7: -------------------------------------------------
    // Get the data from the query in step 6 -------------------

    IWbemClassObject *pclsObj;
    ULONG uReturn = 0;

    while (pEnumerator)
    {
        HRESULT hr = pEnumerator->Next(WBEM_INFINITE, 1,
            &pclsObj, &uReturn);

        if(0 == uReturn)
        {
            break;
        }

        VARIANT Manufacturer;
        VARIANT SMBIOSBIOSVersion;
        VARIANT IdentificationCode;
        VARIANT SerialNumber;
        VARIANT ReleaseDate;
        VARIANT Version;

        // Get the value of the Name property

        hr = pclsObj->Get(L"Manufacturer", 0, &Manufacturer , 0, 0);
        outData += QString("  Manufacturer:%1\n").arg(Manufacturer.bstrVal);
        hr = pclsObj->Get(L"SMBIOSBIOSVersion", 0, &SMBIOSBIOSVersion , 0, 0);
        outData += QString("  SMBIOSBIOSVersion:%1\n").arg(SMBIOSBIOSVersion.bstrVal);
        hr = pclsObj->Get(L"IdentificationCode", 0, &IdentificationCode , 0, 0);
        outData += QString("  IdentificationCode:%1\n").arg(IdentificationCode.bstrVal);
        hr = pclsObj->Get(L"SerialNumber", 0, &SerialNumber , 0, 0);
        outData += QString("  SerialNumber:%1\n").arg(SerialNumber.bstrVal);
        hr = pclsObj->Get(L"ReleaseDate", 0, &ReleaseDate , 0, 0);
        outData += QString("  ReleaseDate:%1\n").arg(ReleaseDate.ulVal);
        hr = pclsObj->Get(L"Version", 0, &Version , 0, 0);
        outData += QString("  Version:%1\n").arg(Version.bstrVal);

        VariantClear(&Manufacturer);
        VariantClear(&SMBIOSBIOSVersion);
        VariantClear(&IdentificationCode);
        VariantClear(&SerialNumber);
        VariantClear(&ReleaseDate);
        VariantClear(&Version);
    }

    pEnumerator->Release();
    pclsObj->Release();
    return outData;
}

QString BaseBoard(HRESULT hres, IEnumWbemClassObject *pEnumerator){

//    HRESULT hres;
//    // Step 6: --------------------------------------------------
//    // Use the IWbemServices pointer to make requests of WMI ----

//    // For example, get the name of the operating system
//    IEnumWbemClassObject* pEnumerator = NULL;
//    hres = pSvc->ExecQuery(
//        bstr_t("WQL"),
//        bstr_t("SELECT * FROM Win32_BaseBoard"),
//        WBEM_FLAG_FORWARD_ONLY | WBEM_FLAG_RETURN_IMMEDIATELY,
//        NULL,
//        &pEnumerator);

//    if (FAILED(hres))
//    {
//        qDebug() << "Query for operating system name failed."
//            << " Error code = 0x"
//            << hex << hres << endl;
//        pSvc->Release();
//        CoUninitialize();
//        return "";               // Program has failed.
//    }
    QString outData = "BaseBoard:\n";

    // Step 7: -------------------------------------------------
    // Get the data from the query in step 6 -------------------

    IWbemClassObject *pclsObj;
    ULONG uReturn = 0;

    while (pEnumerator)
    {
        HRESULT hr = pEnumerator->Next(WBEM_INFINITE, 1,
            &pclsObj, &uReturn);

        if(0 == uReturn)
        {
            break;
        }

        VARIANT Model;
        VARIANT Manufacturer;
        VARIANT Name;
        VARIANT SerialNumber;

        // Get the value of the Name property

        hr = pclsObj->Get(L"Model", 0, &Model , 0, 0);
        outData += QString("  Model:%1\n").arg(Model.bstrVal);
        hr = pclsObj->Get(L"Manufacturer", 0, &Manufacturer , 0, 0);
        outData += QString("  Manufacturer:%1\n").arg(Manufacturer.bstrVal);
        hr = pclsObj->Get(L"Name", 0, &Name , 0, 0);
        outData += QString("  Name:%1\n").arg(Name.bstrVal);
        hr = pclsObj->Get(L"SerialNumber", 0, &SerialNumber , 0, 0);
        outData += QString("  SerialNumber:%1\n").arg(SerialNumber.bstrVal);

        VariantClear(&Model);
        VariantClear(&Manufacturer);
        VariantClear(&Name);
        VariantClear(&SerialNumber);
    }

    pEnumerator->Release();
    pclsObj->Release();
    return outData;
}
