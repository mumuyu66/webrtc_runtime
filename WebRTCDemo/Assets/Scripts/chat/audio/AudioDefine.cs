using UnityEngine;
using System.Collections;

namespace Module.Audio
{
	public class AudioDefine 
	{
		public static int LOOP_RECORD_ID = 1;
		public static int ONCE_RECORD_ID = 2;

		//8k
		public static int FREQUENCY = 8000;
		// freame
		public static int G_FRAME_SIZE = 160;
		public static int G_FRAME_SIZE_ENC = 13;
		public static int S_LOOP_BUFF_SIZE = 20;


		// shot <==> float
		public const int RESCALE_FACTOR = 32767;

		// msInSndCardBuf
		#if UNITY_ANDROID
		public static int MS_IN_SEND_CARDBUF = 90;
		#else
		public static int MS_IN_SEND_CARDBUF = 60;
		#endif
		//设置降噪的力度,0,1,2, 0最弱,2最强
		public static int NS_POLICY = 2;
		// 回声消除模式 0, 1, 2, 3 (default), 4
		public static int ECHO_MODE = 3;

		// mic buff time len 
		public static int MIC_BUFF_LEN = 10;
		// mic delay time len 
		public static float DELAY_TIME = 1f;

		public static int DEBUG_NO = 4;

		public static void ToFArray(float[] result,short[] sa)
		{
			int len = Mathf.Min(result.Length, sa.Length);
			for (int i = 0; i < len; i++)
			{
				result[i] = (float)sa[i] / RESCALE_FACTOR;
			}
		}

		public static void To16Array(short[] result, float[] fa)
		{
			int len = Mathf.Min(result.Length, fa.Length);
			for (int i = 0; i < len; i++) {
				result[i] = (short)(fa[i] * RESCALE_FACTOR);
			}
		}
	}
}