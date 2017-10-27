using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Module.Audio;
using chat;

public class Main : MonoBehaviour
{
	public AudioSource audioSource;
	public AudioClip backMusic;
	public CustomLine[] lines;
	public AudioSource musicAudioSource;

	public Text _delayText;
	public Text logText;

	private AudioRecord _record;
	private ChatManager _chatManager;
	void Start (){
		init ();
	}

	private bool _init = false;
	private AudioStream _stream;
	private void init(){
		if (_init)
			return;
		_stream = new AudioStream (AudioDefine.FREQUENCY * 10 ,AudioDefine.FREQUENCY,AudioDefine.FREQUENCY * 10);
		AudioClip clip =  AudioClip.Create("sadfsf",80000,1,8000,false);
		_chatManager = new ChatManager (_stream,clip);
		_record = new AudioRecord ();
		_record.Init (_chatManager);
		audioSource.clip = _stream.Clip;
//		audioSource.clip = clip;
		audioSource.Play ();
		_init = true;
	}

	private bool _recording = false;
	public void StartMicrophone(){
		AudioListener.volume = 0.4f;
		_record.StartRecord (AudioDefine.LOOP_RECORD_ID);
		_recording = true;
	}

	public void EndMicrophone(){
		AudioListener.volume = 1f;
		_recording = false;
		_record.EndRecord ();
	}

	public void StopMusic(){
		musicAudioSource.Stop ();
	}

	public void PlayMusic(){
		musicAudioSource.Play ();
	}

	public void DelayAdd(){
		AudioDefine.MS_IN_SEND_CARDBUF += 5;
		_delayText.text = AudioDefine.MS_IN_SEND_CARDBUF.ToString ();
	}

	public void DelayRes(){
		AudioDefine.MS_IN_SEND_CARDBUF -= 5;
		_delayText.text = AudioDefine.MS_IN_SEND_CARDBUF.ToString ();
	}

	public void Resample48khzTo8khzAndPlay(){
		//		float[] s_data = new float[(int)(Mathf.CeilToInt(backMusic.length) * 84000)];
		//		backMusic.GetData (s_data,0);
		//		float[] o_data = AudioParse.Parse (s_data);
		//		clip = AudioClip.Create ("11", o_data.Length, 2, 8000,false);
		//		clip.SetData (o_data,0);
		//		musicAudioSource.clip = clip;
		//		musicAudioSource.Play ();
	}

	void FixedUpdate(){
		if (_recording) {
			_record.OnFixedUpdate ();
		}
	}
}