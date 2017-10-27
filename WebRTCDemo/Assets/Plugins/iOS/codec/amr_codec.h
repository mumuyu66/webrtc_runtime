#ifndef _AMR_CODEC_
#define _AMR_CODEC_
#include "codec_def.h"

extern void EXPORT_API amr_init( int mode );

extern void EXPORT_API amr_enc(short *, int );
extern int EXPORT_API next_enc(unsigned char **);

//decode
extern void EXPORT_API amr_dec( unsigned char * , int );
extern int EXPORT_API next_dec(short **);

//exit
extern void EXPORT_API amr_exit();

#endif
