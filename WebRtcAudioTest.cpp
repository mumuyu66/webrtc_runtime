// WebRtcAudioTest.cpp : Defines the entry point for the console application.
//

#include "webrtc/modules/audio_processing/aec/echo_cancellation.h"
#include "webrtc/modules/audio_processing/agc/legacy/gain_control.h"
#include "webrtc/modules/audio_processing/ns/noise_suppression.h"
#include "webrtc/common_audio/signal_processing/include/signal_processing_library.h"

using namespace webrtc;

/*
void WebRtcNS32KSample(char *szFileIn,char *szFileOut,int nSample,int nMode)
{
	int nRet = 0;
	NsHandle *pNS_inst = NULL;

	FILE *fpIn = NULL;
	FILE *fpOut = NULL;

	char *pInBuffer =NULL;
	char *pOutBuffer = NULL;

	do
	{
		int i = 0;
		int nFileSize = 0;
		int nTime = 0;
		WebRtcNs_Create(&pNS_inst);
		WebRtcNs_Init(pNS_inst,nSample);
		WebRtcNs_set_policy(pNS_inst,nMode);

		fpIn = fopen(szFileIn, "rb");
		fpOut = fopen(szFileOut, "wb");
		if (NULL == fpIn || NULL == fpOut)
		{
			printf("open file err \n");
			break;
		}

		fseek(fpIn,0,SEEK_END);
		nFileSize = ftell(fpIn); 
		fseek(fpIn,0,SEEK_SET); 

		pInBuffer = (char*)malloc(nFileSize);
		memset(pInBuffer,0,nFileSize);
		fread(pInBuffer, sizeof(char), nFileSize, fpIn);

		pOutBuffer = (char*)malloc(nFileSize);
		memset(pOutBuffer,0,nFileSize);

		int  filter_state1[6],filter_state12[6];
		int  Synthesis_state1[6],Synthesis_state12[6];

		memset(filter_state1,0,sizeof(filter_state1));
		memset(filter_state12,0,sizeof(filter_state12));
		memset(Synthesis_state1,0,sizeof(Synthesis_state1));
		memset(Synthesis_state12,0,sizeof(Synthesis_state12));

		for (i = 0;i < nFileSize;i+=640)
		{
			if (nFileSize - i >= 640)
			{
				short shBufferIn[320] = {0};
				short shInL[160],shInH[160];
				short shOutL[160] = {0},shOutH[160] = {0};
				memcpy(shBufferIn,(char*)(pInBuffer+i),320*sizeof(short));
				//首先需要使用滤波函数将音频数据分高低频，以高频和低频的方式传入降噪函数内部
				WebRtcSpl_AnalysisQMF(shBufferIn,shInL,shInH,filter_state1,filter_state12);
				//将需要降噪的数据以高频和低频传入对应接口，同时需要注意返回数据也是分高频和低频
				if (0 == WebRtcNs_Process(pNS_inst ,shInL  ,shInH ,shOutL , shOutH))
				{
					short shBufferOut[320];
					//如果降噪成功，则根据降噪后高频和低频数据传入滤波接口，然后用将返回的数据写入文件
					WebRtcSpl_SynthesisQMF(shOutL,shOutH,shBufferOut,Synthesis_state1,Synthesis_state12);
					memcpy(pOutBuffer+i,shBufferOut,320*sizeof(short));
				}
			}	
		}

		fwrite(pOutBuffer, sizeof(char), nFileSize, fpOut);
	} while (0);

	WebRtcNs_Free(pNS_inst);
	fclose(fpIn);
	fclose(fpOut);
	free(pInBuffer);
	free(pOutBuffer);
}

void WebRtcAgcTest(char *filename, char *outfilename,int fs)
{
	FILE *infp      = NULL;
	FILE *outfp     = NULL;

	short *pData    = NULL;
	short *pOutData = NULL;
	void *agcHandle = NULL;	

	do 
	{
		WebRtcAgc_Create(&agcHandle);

		int minLevel = 0;
		int maxLevel = 255;
		int agcMode  = kAgcModeFixedDigital;
		WebRtcAgc_Init(agcHandle, minLevel, maxLevel, agcMode, fs);

		WebRtcAgc_config_t agcConfig;
		agcConfig.compressionGaindB = 20;
		agcConfig.limiterEnable     = 1;
		agcConfig.targetLevelDbfs   = 3;
		WebRtcAgc_set_config(agcHandle, agcConfig);

		infp = fopen(filename,"rb");
		int frameSize = 80;
		pData    = (short*)malloc(frameSize*sizeof(short));
		pOutData = (short*)malloc(frameSize*sizeof(short));

		outfp = fopen(outfilename,"wb");
		int len = frameSize*sizeof(short);
		int micLevelIn = 0;
		int micLevelOut = 0;
		while(TRUE)
		{
			memset(pData, 0, len);
			len = fread(pData, 1, len, infp);
			if (len > 0)
			{
				int inMicLevel  = micLevelOut;
				int outMicLevel = 0;
				uint8_t saturationWarning;
				int nAgcRet = WebRtcAgc_Process(agcHandle, pData, NULL, frameSize, pOutData,NULL, inMicLevel, &outMicLevel, 0, &saturationWarning);
				if (nAgcRet != 0)
				{
					printf("failed in WebRtcAgc_Process\n");
					break;
				}
				micLevelIn = outMicLevel;
				fwrite(pOutData, 1, len, outfp);
			}
			else
			{
				break;
			}
		}
	} while (0);

	fclose(infp);
	fclose(outfp);
	free(pData);
	free(pOutData);
	WebRtcAgc_Free(agcHandle);
}
*/
#define  NN 160

static short temp[NN];

int fread_f(float* buf, FILE* fptr) {
    int size = fread(temp, sizeof(int16_t), NN, fptr);
    if (size < 0) {
        memset(buf, 0, sizeof(float) * NN);
        return size;
    }
    if (NN > size) {
        memset(buf+size, 0, sizeof(float) * (NN-size));
    }
    for (size_t i = 0; i < size; ++ i) {
        *buf++ = temp[i] ;
    }
    return size;
}



int WebRtcAecTest(char* speaker, char* mic, char* output)
{
	float far_frame[NN];
	float near_frame[NN];
	float out_frame[NN];

	void *aecmInst = NULL;
	FILE *fp_far  = fopen(speaker, "rb");
	FILE *fp_near = fopen(mic, "rb");
	FILE *fp_out  = fopen(output, "wb");

	do 
	{
		if(!fp_far || !fp_near || !fp_out)
		{
			printf("WebRtcAecTest open file err \n");
			break;
		}

		aecmInst = WebRtcAec_Create();
		WebRtcAec_Init(aecmInst, 8000, 8000);

		AecConfig config;
		config.nlpMode = kAecNlpConservative;
		WebRtcAec_set_config(aecmInst, config);

		while(1)
		{
			if (NN == fread_f(far_frame, fp_far))
			{
				fread_f(near_frame, fp_near);
				WebRtcAec_BufferFarend(aecmInst, far_frame, NN);

                float* const p = near_frame;
                const float* const*  ptrNear = &p;
                
                float* const q = out_frame;
                float* const* ptrOut = &q;
                
				WebRtcAec_Process(aecmInst, ptrNear, 1, ptrOut, NN,40,0);
                
                for (int i = 0; i < NN; ++ i) {
                    temp[i] = (int16_t)out_frame[i];
                }
                
				fwrite(temp, sizeof(short), NN, fp_out);
			}
			else
			{
				break;
			}
		}
	} while (0);

	fclose(fp_far);
	fclose(fp_near);
	fclose(fp_out);
	WebRtcAec_Free(aecmInst);
	return 0;
}

int main(int argc, char* argv[])
{   
	WebRtcAecTest(argv[1], argv[2], argv[3]);
/*
	WebRtcAgcTest("byby_8K_1C_16bit.pcm","byby_8K_1C_16bit_agc.pcm",8000);

	WebRtcNS32KSample("lhydd_1C_16bit_32K.PCM","lhydd_1C_16bit_32K_ns.pcm",32000,1);
*/
	printf("press any key to continue...\n");

	getchar();
	return 0;
}
