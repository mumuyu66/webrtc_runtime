using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Module.Audio
{
	public class AudioRecord 
	{
		private static float[] g_source160;
		private static float[] o_source160;
		private static short[] o_source;

		private AudioProcessing _processing;
		private IChatManager _chat;
		private Dictionary<int,IRecord> _recodMap;
		private IRecord _currentRecord = null;
		public void Init(IChatManager chat){
			this._chat = chat;
			g_source160 = new float[AudioDefine.G_FRAME_SIZE];
			o_source = new short[AudioDefine.G_FRAME_SIZE];
			initRecords ();
			_processing = new AudioProcessing ();
			_processing.InitAmr ();
		}

		private void initRecords(){
			_recodMap = new Dictionary<int, IRecord>();
			AudioLoopRecord loopRecord = new AudioLoopRecord ();
			loopRecord.Init (AudioDefine.LOOP_RECORD_ID,_chat);
			_recodMap.Add (loopRecord.Id, loopRecord);
			AudioOnceRecord onceRecord = new AudioOnceRecord ();
            onceRecord.Init (AudioDefine.ONCE_RECORD_ID,_chat);
			_recodMap.Add (onceRecord.Id, onceRecord);
		}

		private int _vad = 0;
		private void checkPosition(){
			// 帧间隔超过1秒时  录的声音直接抛弃
			if (Time.realtimeSinceStartup - _lastT < AudioDefine.DELAY_TIME) {
				if ( _currentP < _lastP ) {
					if (_lastP < _maxlen) {
						_currentP = _maxlen;
					} else {
						_lastP = 0;
					}
				}
				while(_currentP - _lastP >= AudioDefine.G_FRAME_SIZE) {
					_mclip.GetData (g_source160,_lastP);
					 _vad = 0;
					if (AudioDefine.DEBUG_NO == 1) {
						o_source160 = _processing.Processing1 (g_source160, ref _vad);
					} else if (AudioDefine.DEBUG_NO == 2) {
						o_source160 = _processing.Processing2 (g_source160, ref _vad);
					} else if (AudioDefine.DEBUG_NO == 3) {
						o_source160 = _processing.Processing3 (g_source160, ref _vad);
					} else if (AudioDefine.DEBUG_NO == 4) {
						o_source160 = _processing.Processing4 (g_source160, ref _vad);
					} else if (AudioDefine.DEBUG_NO == 5) {
						o_source160 = _processing.Processing5 (g_source160, ref _vad);
					}
					else {
						o_source160 = _processing.Processing(g_source160, ref _vad);
					}
					_currentRecord.PushToBuff (o_source160,_vad);
					_lastP += AudioDefine.G_FRAME_SIZE;
				}
			} else {
				_lastP = _currentP;
			}
		}

		private int _lastP = 0;
		private int _currentP = 0;
		private float _lastT = 0f;
		public void OnFixedUpdate(){
			_currentP = Microphone.GetPosition (null);
			_currentP = _currentP - _currentP % AudioDefine.G_FRAME_SIZE;
			checkPosition ();
			_lastT = Time.realtimeSinceStartup;
		}

		public void TryGetMicrophonePermission(){
			string[] devices = Microphone.devices;
		}

		private AudioClip _mclip = null;
		private int _maxlen = 0;
		public void StartRecord(int recordId){
			_currentRecord = _recodMap[recordId];
			TryGetMicrophonePermission ();
			_currentRecord.StartRecord ();
			_maxlen = AudioDefine.FREQUENCY * AudioDefine.MIC_BUFF_LEN;
			_processing.Init ();
			Microphone.End (null);
			_mclip = Microphone.Start (null, true,AudioDefine.MIC_BUFF_LEN, AudioDefine.FREQUENCY);
			AudioProcessingDll.ForceToSpeaker ();
		}

		public void EndRecord(){
			_currentRecord.EndRecord();
			Microphone.End (null);
		}

		public void Clean(){
			_processing.Clean ();
			foreach (IRecord record in _recodMap.Values) {
				record.Clean ();
			}
			_recodMap.Clear ();
		}

	}	
}