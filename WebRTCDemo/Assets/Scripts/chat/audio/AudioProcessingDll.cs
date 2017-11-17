using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace Module.Audio
{
	public class AudioProcessingDll 
	{
		#if UNITY_IOS && !UNITY_EDITOR
		private const string WEBRTC_DLL = "__Internal";
		#else
		private const string WEBRTC_DLL = "webrtc";
		#endif

		[DllImport(WEBRTC_DLL,CallingConvention = CallingConvention.Cdecl)]
		public static extern int WebRtcInit(int nsPolicy, int echoMode, int vadMode);

		[DllImport(WEBRTC_DLL,CallingConvention = CallingConvention.Cdecl)]
		public static extern int WebRtc_BufferFarend( short[] f_buf);

		[DllImport(WEBRTC_DLL,CallingConvention = CallingConvention.Cdecl)]
		public static extern int Process(short[] i_buf,ref IntPtr o_buf,int msInSndCardBuf);

		[DllImport(WEBRTC_DLL,CallingConvention = CallingConvention.Cdecl)]
		public static extern int WebRtc_Resample48khzTo8khz(short[] i_buf,ref IntPtr o_buf);

		[DllImport(WEBRTC_DLL,CallingConvention = CallingConvention.Cdecl)]
		public static extern void WebRtcFree();


		#if UNITY_IOS && !UNITY_EDITOR
		private const string AMR_DLL = "__Internal";
		#else
		private const string AMR_DLL = "codec";
		#endif

		[DllImport(AMR_DLL,CallingConvention = CallingConvention.Cdecl)]
		public static extern int amr_init( int mode );

		[DllImport(AMR_DLL,CallingConvention = CallingConvention.Cdecl)]
		public static extern void amr_enc( short[] i_buf, int len  );

		[DllImport(AMR_DLL,CallingConvention = CallingConvention.Cdecl)]
		public static extern int next_enc(ref IntPtr o_buf);

		[DllImport(AMR_DLL,CallingConvention = CallingConvention.Cdecl)]
		public static extern void amr_dec( byte[] i_buf,  int len );

		[DllImport(AMR_DLL,CallingConvention = CallingConvention.Cdecl)]
		public static extern int next_dec( ref IntPtr o_buf );

		[DllImport(AMR_DLL,CallingConvention = CallingConvention.Cdecl)]
		public static extern void amr_exit();


		#if UNITY_IPHONE
		[DllImport("__Internal")]
		private static extern void _forceToSpeaker();
		#endif

		public static void ForceToSpeaker() {
			#if UNITY_IPHONE
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				_forceToSpeaker();
			}
			#endif
		}
	}

}