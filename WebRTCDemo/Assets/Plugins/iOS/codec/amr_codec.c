#include "amr_codec.h"
#include "interf_dec.h"
#include "interf_enc.h"
#include <stdlib.h>
#include <string.h>

int * en_state;

typedef struct {
	unsigned char *source;
	short de_synth[160];
	int   len;
	int   current;
} decode_session;

typedef struct {
	short *source;
	unsigned char en_data[32];
	int len;
	int current;
} encode_session ;

short block_size[16] = { 12, 13, 15, 17, 19, 20, 26, 31, 5, 0, 0, 0, 0, 0, 0, 0 };
int * de_state;
decode_session dec_session;
encode_session enc_session;

enum Mode enc_mode;


void EXPORT_API amr_init( int mode ) {
	enc_mode = (enum Mode)mode;
	memset(&enc_session, 0, sizeof(encode_session));
	memset(&dec_session, 0, sizeof(decode_session));

	en_state = (int *)Encoder_Interface_init(0);
	de_state = (int *)Decoder_Interface_init();
}

void EXPORT_API amr_enc(short *src, int len) {
	enc_session.len = len;
	enc_session.current = 0;
	enc_session.source = src;
}

int EXPORT_API next_enc(unsigned char **dest) {
	if (enc_session.current >= enc_session.len)
	{
		return -1;
	}
	int block = Encoder_Interface_Encode((void *)en_state, enc_mode, &enc_session.source[enc_session.current], enc_session.en_data, 0);
	*dest = enc_session.en_data;
	enc_session.current += 160;
	return block;
}

void EXPORT_API amr_dec(unsigned char *src, int len) {
	dec_session.len = len;
	dec_session.current = 0;
	dec_session.source = src;

}

int EXPORT_API next_dec(short **dest) {
	if (dec_session.current >= dec_session.len)
	{
		return -1;
	}
	enum Mode mode = (dec_session.source[dec_session.current]) >> 3 & 0x0F;
	short block = block_size[mode];
	Decoder_Interface_Decode((void*)de_state, &dec_session.source[dec_session.current], dec_session.de_synth, 0);
	*dest = dec_session.de_synth;
	dec_session.current = dec_session.current + block + 1;
	return 160;
}

void EXPORT_API amr_exit() {
	memset(&enc_session, 0, sizeof(encode_session));
	memset(&dec_session, 0, sizeof(decode_session));

	Encoder_Interface_exit(en_state);
	Decoder_Interface_exit(de_state);

	en_state = NULL;
	de_state = NULL;
}
