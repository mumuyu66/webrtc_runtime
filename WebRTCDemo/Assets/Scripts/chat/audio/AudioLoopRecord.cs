using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Module.Audio
{
	public class AudioLoopRecord :IRecord
	{
		private static byte[] _sendByteBuff = new byte[AudioDefine.G_FRAME_SIZE_ENC*AudioDefine.S_LOOP_BUFF_SIZE];
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

		public void EndRecord(){
		}

		private int _sendIndex = 0;
		private bool _hasData = false;
		public void PushToBuff(float[] source,int vad){
			if (vad >= 0) {
				byte[] enbyte = AudioProcessing.EnFrame (source);
				Array.Copy (enbyte,0,_sendByteBuff,_sendIndex * AudioDefine.G_FRAME_SIZE_ENC,AudioDefine.G_FRAME_SIZE_ENC);
				_hasData = true;
			}
			if ( ++_sendIndex >= AudioDefine.S_LOOP_BUFF_SIZE) {
				if (_hasData) {
					_chat.SendVoidMessage (_sendByteBuff,1);
				}
				resetBuff ();
			}
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