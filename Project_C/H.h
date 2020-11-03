#pragma once
#ifdef DLLC_EXPORTS
#define DLLC_API __declspec(dllexport)
#else
#define DLLC_API __declspec(dllimport)
#endif



extern "C" DLLC_API void LightenC(unsigned char* tab, unsigned char val);
extern "C" DLLC_API void DimC(unsigned char* tab, unsigned char val); 
extern "C" DLLC_API void BlendC(unsigned char* tab, unsigned char val); 
