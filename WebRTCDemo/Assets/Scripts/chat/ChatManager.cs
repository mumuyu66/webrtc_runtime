using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Module.Audio;

namespace chat
{
	public class ChatManager :IChatManager
	{
		private AudioStream _stream;
		private AudioClip _clip;
		public ChatManager(AudioStream stream,AudioClip clip){
			_stream = stream;
			_clip = clip;
		}

		private float[] _buff = new float[80000];
		private int _index = 0;
		public void  SendVoidMessage (byte[] data, int len){
			float[] enbuff = AudioProcessing.DeFrames (data);
			for(int i=0;i<enbuff.Length;i++){
				enbuff [i] *= 2;
			}
			_stream.Input (enbuff);
		}

		public void SendFriendMessage (byte[] data, int len,int dataLen){
		}
	}	
}
