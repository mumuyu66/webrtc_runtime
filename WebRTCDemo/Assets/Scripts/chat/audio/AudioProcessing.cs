using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Module.Audio
{
	public class AudioProcessing 
	{
		private static float[] o_source160;

		private static short[] g_source;
		private static short[] o_source;

		private bool _isInit = false;
		public void Init(){
			if (_isInit) {
				return;
			}
			o_source160 = new float[AudioDefine.G_FRAME_SIZE];
			g_source = new short[AudioDefine.G_FRAME_SIZE];
			o_source = new short[AudioDefine.G_FRAME_SIZE];
			AudioProcessingDll.WebRtcInit (AudioDefine.NS_POLICY,AudioDefine.ECHO_MODE,AudioDefine.VAD_MODE);

			_isInit = true;
		}

		private bool _amrIsInit = false;
		public void InitAmr(){
			if (_amrIsInit) {
				return;
			}
			AudioProcessingDll.amr_init (0);
			_amrIsInit = true;
		}

		private int _delayFreamSize = 1;
		public float[] Processing(float[] audioSource,ref int vad){
			if (!_isInit) {
				return o_source160;
			}
			AudioDefine.To16Array (g_source,audioSource);
			if (_delayFreamSize <= 0) {
				IntPtr ptr = IntPtr.Zero;
				vad = AudioProcessingDll.Process(g_source,ref ptr,AudioDefine.MS_IN_SEND_CARDBUF);
				Marshal.Copy(ptr,o_source,0,AudioDefine.G_FRAME_SIZE);
				AudioDefine.ToFArray (o_source160,o_source);
			} else {
				_delayFreamSize--;
			}
			AudioProcessingDll.WebRtc_BufferFarend (g_source);
			return o_source160;
		}

		public float[] Processing1(float[] audioSource,ref int vad){
			if (!_isInit) {
				return o_source160;
			}
			if (_delayFreamSize <= 0) {

				AudioDefine.To16Array (o_source,audioSource);
				AudioProcessingDll.WebRtc_BufferFarend (o_source);

				IntPtr ptr = IntPtr.Zero;
				vad = AudioProcessingDll.Process(g_source,ref ptr,AudioDefine.MS_IN_SEND_CARDBUF);
				for (int i = 0; i < g_source.Length; i++) {
					g_source [i] = o_source [i];
				}
				Marshal.Copy(ptr,o_source,0,AudioDefine.G_FRAME_SIZE);
				AudioDefine.ToFArray (o_source160,o_source);
			} else {
				_delayFreamSize--;
				AudioDefine.To16Array (g_source,audioSource);
			}

			return o_source160;
		}

		public float[] Processing2(float[] audioSource,ref int vad){
			return audioSource;
		}

		private static float[] i_data = null;
		private static int index = 0;
		public static void SetBuffData(float[] data){
			i_data = data;
			index = 0;
		}

		public static short[] GetBuffFarend(){
			if (index + AudioDefine.G_FRAME_SIZE <= i_data.Length) {
				for (int i = 0; i < AudioDefine.G_FRAME_SIZE; i++) {
					o_source [i] = (short)(i_data [i + index] * 32767);
				}
				index += AudioDefine.G_FRAME_SIZE;
			}
			return o_source;
		}

		public float[] Processing3(float[] audioSource,ref int vad){
			if (!_isInit || i_data == null) {
				return audioSource;
			}
			AudioDefine.To16Array (g_source,audioSource);
			IntPtr ptr = IntPtr.Zero;

			AudioProcessingDll.WebRtc_BufferFarend (GetBuffFarend());
			vad = AudioProcessingDll.Process(g_source,ref ptr,AudioDefine.MS_IN_SEND_CARDBUF);
			Marshal.Copy(ptr,o_source,0,AudioDefine.G_FRAME_SIZE);
			AudioDefine.ToFArray (o_source160,o_source);

			return o_source160;
		}

		private static float[] f_listenerBuff = new float[1024];
		private static short[] s960_listenerBuff = new short[960];
		public static short[] GetListerBuff(){
			IntPtr ptr = IntPtr.Zero;

			AudioListener.GetOutputData (f_listenerBuff,0);
			int index = 0;

			for (int i = 0; i < s960_listenerBuff.Length; i++) {
				s960_listenerBuff [i] = (short)(f_listenerBuff [i+64] * AudioDefine.RESCALE_FACTOR);
			}

			AudioProcessingDll.WebRtc_Resample48khzTo8khz (s960_listenerBuff,ref ptr);
			Marshal.Copy(ptr,o_source,0,AudioDefine.G_FRAME_SIZE);

			return o_source;
		}

		public float[] Processing4(float[] audioSource,ref int vad){
			if (!_isInit || i_data == null) {
				return audioSource;
			}
			AudioDefine.To16Array (g_source,audioSource);
			IntPtr ptr = IntPtr.Zero;

			AudioProcessingDll.WebRtc_BufferFarend (GetListerBuff());
			vad = AudioProcessingDll.Process(g_source,ref ptr,AudioDefine.MS_IN_SEND_CARDBUF);
			Marshal.Copy(ptr,o_source,0,AudioDefine.G_FRAME_SIZE);
			AudioDefine.ToFArray (o_source160,o_source);

			return o_source160;
		}

		public float[] Processing5(float[] audioSource,ref int vad){
			if (!_isInit || i_data == null) {
				return audioSource;
			}

			AudioDefine.ToFArray (o_source160,o_source);

			return o_source160;
		}

		private static byte[] outByte = new byte[AudioDefine.G_FRAME_SIZE_ENC];
		public static byte[] EnFrame(float[] ibuf){
			AudioDefine.To16Array (outShort, ibuf);
			AudioProcessingDll.amr_enc (outShort,ibuf.Length);
			IntPtr ptr = IntPtr.Zero;
			int len = AudioProcessingDll.next_enc(ref ptr);
			Marshal.Copy(ptr,outByte,0,len);
			return outByte;
		}

		private static short[] outShort = new short[AudioDefine.S_LOOP_BUFF_SIZE * AudioDefine.G_FRAME_SIZE]; 
		private static float[] outfloat = new float[AudioDefine.S_LOOP_BUFF_SIZE * AudioDefine.G_FRAME_SIZE];
		public static float[] DeFrames(byte[] ibuf){
			AudioDefine.ToFArray(outfloat,DeFramesOutShot(ibuf));
			return outfloat;
		}

		public static short[] DeFramesOutShot(byte[] ibuf){
			IntPtr ptr = IntPtr.Zero;
			int pos = 0;
			AudioProcessingDll.amr_dec (ibuf,ibuf.Length);
			int len = AudioProcessingDll.next_dec (ref ptr);
			while (len != -1 ) {
				Marshal.Copy(ptr,outShort,pos,len);
				pos += len;
				len = AudioProcessingDll.next_dec(ref ptr);
			}
			return outShort;
		}

		public void Clean(){
			if (_isInit) {
				AudioProcessingDll.WebRtcFree ();
				_isInit = false;
			}
			if (_amrIsInit) {
				AudioProcessingDll.amr_exit ();
				_amrIsInit = false;
			}
		}
	}
}