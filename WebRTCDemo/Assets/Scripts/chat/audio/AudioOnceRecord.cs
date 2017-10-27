using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Module.Audio
{
	public class AudioOnceRecord :IRecord
	{
		// buff = 30s
		private static byte[] _sendByteBuff = new byte[19500];
		private static int _sendBuff = 1500;
		private IChatManager _chat;

		private int _id;
		public int Id{
			get{ return _id;}
		}

		public void Init(int id,IChatManager chat){
			_id = id;
			_chat = chat;
		}

		public void StartRecord(){
			resetBuff ();
		}

        public void EndRecord()
        {
			_chat.SendFriendMessage(_sendByteBuff,  Mathf.CeilToInt(_sendIndex /50.0f),_sendIndex * AudioDefine.G_FRAME_SIZE_ENC);
        }

        public void StopRecord()
        {
            _sendByteBuff = new byte[19500];
        }
			
		private int _sendIndex = 0;
		private bool _hasData = false;
		public void PushToBuff(float[] source,int vad){
			if (vad >= 0) {
				byte[] enbyte = AudioProcessing.EnFrame (source);
				Array.Copy (enbyte,0,_sendByteBuff,_sendIndex * AudioDefine.G_FRAME_SIZE_ENC,AudioDefine.G_FRAME_SIZE_ENC);
				_hasData = true;
			}
			if ( ++_sendIndex >= _sendBuff) {
				if (_hasData) {
					_chat.SendFriendMessage (_sendByteBuff,30,_sendIndex * AudioDefine.G_FRAME_SIZE_ENC);
				}
				resetBuff ();
			}
		}

		private static short[] outShort = new short[240000]; 
		private static float[] outfloat = new float[240000];
		public static float[] DeFrames(byte[] ibuf,ref int index){
			AudioDefine.ToFArray(outfloat,DeFramesOutShot(ibuf,ref index));
			return outfloat;
		}

		public static short[] DeFramesOutShot(byte[] ibuf, ref int index){
			IntPtr ptr = IntPtr.Zero;
			int pos = 0;
			AudioProcessingDll.amr_dec (ibuf,ibuf.Length);
			int len = AudioProcessingDll.next_dec (ref ptr);
			while (len != -1 ) {
				Marshal.Copy(ptr,outShort,pos,len);
				pos += len;
				len = AudioProcessingDll.next_dec(ref ptr);
			}
			index = pos;
			return outShort;
		}

		private void resetBuff(){
			for (int i = 0; i < _sendByteBuff.Length; i++) {
				_sendByteBuff [i] = 0;
			}
			_hasData = false;
			_sendIndex = 0;
		}

		public void Clean(){
			
		}
	}
}