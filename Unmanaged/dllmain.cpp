// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"

bool __stdcall SetConfiguration(void* rawConfig, unsigned int rawConfigBytes)
{
    return true;
}

bool __stdcall SetConfigurationRaw(void* rawConfig, unsigned int rawConfigBytes)
{
    return true;
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

