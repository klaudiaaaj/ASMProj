#include "H.h"
#include "pch.h"
#include <stdio.h>
#include <emmintrin.h>

void BlendC(unsigned char* tab, unsigned char val)
{

	__m128i* l = (__m128i*)tab;					// za³adowanie tablicy 16 pikseli do rejestru xmm l
	__m128i* r = &_mm_set1_epi8(val);			// wype³nienie  rejestru xmm r 16 wartoœciami val(wspolczynnik zmiany jasnosci)
	_mm_store_si128(l, _mm_subs_epu8(*l, *r));	// zsumowanie dwóch rejestrów xmm (r+l), a nastêpnie zapisanie ich w xmm l
	return;
}