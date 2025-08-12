#include "filehelper.h"
#include <iostream>

namespace QtSystemLib {

std::string FileHelper::GetFileVersion(std::string path)
{
    std::string result;
    std::wstring widestr = std::wstring(path.begin(), path.end());
    const wchar_t *widecstr = widestr.c_str();

    DWORD verHandle = 0;
    UINT size = 0;
    LPBYTE lpBuffer = NULL;
    DWORD verSize = GetFileVersionInfoSize(widecstr, &verHandle);

    if (verSize != NULL) {
        LPSTR verData = new char[verSize];

        if (GetFileVersionInfo(widecstr, verHandle, verSize, verData)) {
            if (VerQueryValueA(verData, "\\", (VOID FAR * FAR *) &lpBuffer, &size)) {
                if (size) {
                    VS_FIXEDFILEINFO *verInfo = (VS_FIXEDFILEINFO *) lpBuffer;
                    if (verInfo->dwSignature == 0xfeef04bd) {
                        std::stringstream ss;
                        ss << (verInfo->dwFileVersionMS >> 16) << ".";
                        ss << (verInfo->dwFileVersionMS >> 0) << ".";
                        ss << (verInfo->dwFileVersionLS >> 16) << ".";
                        ss << (verInfo->dwFileVersionLS >> 0);
                        result = ss.str();
                    }
                }
            }
        }
        delete[] verData;
    }
    return result;
}
} // namespace QtSystemLib
