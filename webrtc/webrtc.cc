#include "webrtc/modules/audio_processing/aec/echo_cancellation.h"
#include "webrtc/modules/audio_processing/agc/legacy/gain_control.h"
#include "webrtc/modules/audio_processing/ns/noise_suppression.h"
#include "webrtc/modules/audio_processing/ns/noise_suppression_x.h"
#include "webrtc/common_audio/signal_processing/include/signal_processing_library.h"
#include "webrtc/modules/audio_processing/aecm/echo_control_mobile.h"
#include "webrtc/common_audio/vad/include/webrtc_vad.h"
#include "webrtc/modules/audio_processing/agc/legacy/analog_agc.h"
#include "webrtc/common_audio/signal_processing/resample_by_2_internal.h"

extern "C" {
#include "webrtc/codec/codec_def.h"
#include "webrtc/codec/interf_dec.h"
#include "webrtc/codec/interf_enc.h"
}

#if defined(_WIN32) || defined (__WIN32__) || defined (WIN32)
#define EXPORT_API extern "C" __declspec(dllexport)
#else 
#define EXPORT_API extern "C" 
#endif

#define  NN 160
#define  SAMPLE_RATE 8000

using namespace webrtc;

NsxHandle* NSX_instance = NULL;
void* ACE_aecm = NULL;
VadInst* Vad_instance = NULL;
void* Agc_instance = NULL;

EXPORT_API int WebRtcInit(int nsPolicy,int echoMode){
	int ret = 0;
	ACE_aecm = WebRtcAecm_Create();
	ret = WebRtcAecm_Init(ACE_aecm,SAMPLE_RATE);
	AecmConfig aecmConfig;
	aecmConfig.cngMode = AecmTrue;
	aecmConfig.echoMode = echoMode;
	ret = WebRtcAecm_set_config(ACE_aecm,aecmConfig);
	
	//------------------------------------------------------------------------------
    //创建WebRtcNs实例
    NSX_instance = WebRtcNsx_Create();

    //初始化WebRtcNs实例,此处需要指定采样,告诉它一次可以处理多少个short音频数据,
    //如果是8000, 则一次可以处理80,如果是44100, 则一次可以处理441个
    //也就是说,一次性可以处理10ms时间的数据
    if ((ret = WebRtcNsx_Init(NSX_instance, SAMPLE_RATE) )) {
        return ret;
    }

    //设置降噪的力度,0,1,2, 0最弱,2最强
    if ( ( ret =  WebRtcNsx_set_policy(NSX_instance, nsPolicy) ) ){
        return ret;
    }
	
	//-------------------------------------------------------------------------------
	Vad_instance = WebRtcVad_Create();
	WebRtcVad_Init(Vad_instance);
	
	//-------------------------------------------------------------------------------
	Agc_instance = WebRtcAgc_Create();
	WebRtcAgc_Init(Agc_instance,0,255,3,SAMPLE_RATE);
	WebRtcAgcConfig agcConfig;
	agcConfig.compressionGaindB = 20;
	agcConfig.limiterEnable     = 1;
	agcConfig.targetLevelDbfs   = 3;
	ret = WebRtcAgc_set_config(Agc_instance, agcConfig);

    return ret;
}

EXPORT_API void WebRtc_BufferFarend(const int16_t* far_frame){
	WebRtcAecm_BufferFarend(ACE_aecm, far_frame, NN);
}

static int16_t in[160] = {0};
static int16_t out[160] = {0};
EXPORT_API int Process(int16_t* const spframe , int16_t** const outframe,int16_t msInSndCardBuf){
	
	for(int i=0;i<160; i += 80){
		int16_t* const pn = spframe + i;
		const int16_t* const* ptrNearn = &pn;
		
		int16_t* const qn = in + i;
		int16_t* const* ptrOutn = &qn;

		WebRtcNsx_Process(NSX_instance ,ptrNearn ,1 ,ptrOutn);
	}
	
	const int16_t* cint = in;
	WebRtcAecm_Process(ACE_aecm, spframe,cint, out, NN,msInSndCardBuf);
	*outframe = out;
	int v = WebRtcVad_Process(Vad_instance,SAMPLE_RATE,*outframe,NN);
	if(v > 0){
			for(int i=0;i<160; i += 80){
			int16_t* const pn = out + i;
			const int16_t* const* ptrNearn = &pn;
			
			int16_t* const qn = in + i;
			int16_t* const* ptrOutn = &qn;
			uint8_t saturationWarning;
			int32_t inMicLevel = 0;
			WebRtcAgc_Process(Agc_instance ,ptrNearn ,1 ,80,ptrOutn,inMicLevel,&inMicLevel,0,&saturationWarning);
		}
		*outframe = in;
	}
	
	return v;
}

WebRtcSpl_State48khzTo8khz WebRtc48khzTo8khzstate;
EXPORT_API void WebRtc_Resample48khzTo8khz(const int16_t* in, int16_t** out) {
  size_t i;
  int16_t speech_nb[160];  // 20 ms in 8 kHz.
  // |tmp_mem| is a temporary memory used by resample function, length is
  // frame length in 10 ms (480 samples) + 256 extra.
  int32_t tmp_mem[480 + 256] = { 0 };
  const size_t kFrameLen10ms8khz = 80;
  const size_t kFrameLen10ms48khz = 480;
  for (i = 0; i < 2; i++) {
    WebRtcSpl_Resample48khzTo8khz(&in[i * kFrameLen10ms48khz],
                                  &speech_nb[i * kFrameLen10ms8khz],
                                  &WebRtc48khzTo8khzstate,
                                  tmp_mem);
  }
  *out = speech_nb;
}

EXPORT_API void WebRtcFree(){
    //释放WebRtc资源
    WebRtcNsx_Free(NSX_instance);
	WebRtcAecm_Free(ACE_aecm);
	WebRtcVad_Free(Vad_instance);
	WebRtcAgc_Free(Agc_instance);
}