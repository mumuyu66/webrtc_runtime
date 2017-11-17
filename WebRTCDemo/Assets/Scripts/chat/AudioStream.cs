using System;
using UnityEngine;
using Module.Audio;

namespace chat
{
	public class AudioStream
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="chat.AudioStream"/> class.
		/// </summary>
		/// <param name="i_audioLength"> Audio Clip Length .</param>
		/// <param name="i_audioFrequency"> Audio Clip Frequency.</param>
		/// <param name="i_blockSize"> Input Buffer Size </param>
		public AudioStream(int i_audioLength, int i_audioFrequency, int i_bufferSize){
			buffer = new float[i_bufferSize];
			size = i_bufferSize;
			clip = AudioClip.Create ("stream_audio_" + GetHashCode (), i_audioLength, 1, i_audioFrequency, true, OnPCMRead);
		}

		public void Reset(){
			ptrRead = 0;
			ptrWrite = 0;
		}

		private void OnPCMRead(float[] i_data){
			Array.Clear (i_data, 0, i_data.Length);
			ptrRead = Mathf.Max (ptrWrite - size, ptrRead);
			int _samples = Mathf.Min (ptrWrite - ptrRead, i_data.Length);
			int _round2 = (ptrRead + _samples) / size;
			int _round1 = ptrRead / size;
            if (ptrRead == ptrWrite) {
				Array.Clear (i_data, 0, i_data.Length);
				this.PlayEOF = true;
            } else if (_round2 != _round1) {
				int _offset0 = ptrRead % size;
				int _offset1 = (ptrRead + _samples) % size;

				Buffer.BlockCopy (buffer, _offset0 * 4, i_data, 0, (size - _offset0) * 4);

				Buffer.BlockCopy (buffer, 0, i_data, (size - _offset0) * 4, _offset1 );
			} else {
				Buffer.BlockCopy (buffer, ptrRead % size * 4 , i_data, 0, _samples * 4);
			}
			ptrRead += _samples;
			AudioProcessing.SetBuffData (i_data);
		}

		public void Input(float[] i_data){
			int _ptrTo = ptrWrite + i_data.Length;
			int _round2 = _ptrTo / size;
			int _round1 = ptrWrite / size;
			if (i_data.Length >= size) {
				Buffer.BlockCopy (i_data, (i_data.Length - size) * 4, buffer, 0, size * 4);
				ptrRead = 0;
				ptrWrite = size;
			} else {
				int _samples = i_data.Length;
				if (_round2 != _round1) {
					int _offset0 = ptrWrite % size;
					int _offset1 = _ptrTo % size;
					Buffer.BlockCopy (i_data, 0, buffer, _offset0 * 4,  (size - _offset0) * 4);
					Buffer.BlockCopy (i_data, (size - _offset0) * 4, buffer, 0, _offset1 * 4);
				} else {
					Buffer.BlockCopy (i_data, 0, buffer, ptrWrite % size * 4,  _samples * 4);
				}
				ptrWrite += _samples;
				ptrRead = Mathf.Max (ptrWrite - size, ptrRead);

				int _n = ptrRead / size;
				ptrRead -= _n * size;
				ptrWrite -= _n * size;
			}
			PlayEOF = false;
		}

		public void Input<T>(T[] i_data, Func<T, float> convert){
			int _ptrTo = ptrWrite + i_data.Length;
			int _round2 = _ptrTo / size;
			int _round1 = ptrWrite / size;
			if (i_data.Length >= size) {
				for (int _i = 0; _i < size; ++_i) {
					buffer [_i] = convert (i_data [i_data.Length - size + _i]);
				}
				ptrRead = 0;
				ptrWrite = size;
			} else {
				int _samples = i_data.Length;
				if (_round2 != _round1) {
					int _offset0 = ptrWrite % size;
					int _offset1 = _ptrTo % size;
					for (int _i = 0; _i < size - _offset0; ++_i) {
						buffer [_offset0 + _i] = convert (i_data [_i]);
					}

					for (int _i = 0; _i < _offset1; ++_i) {
						buffer [_i] = convert (i_data [size - _offset0 + _i]);
					}
				} else {
					int _offset0 = ptrWrite % size;
					for (int _i = 0; _i < _samples; ++_i) {
						buffer [_offset0 + _i] = convert (i_data [_i]);
					}
				}
				ptrWrite += _samples;
				ptrRead = Mathf.Max (ptrWrite - size, ptrRead);

				int _n = ptrRead / size;
				ptrRead -= _n * size;
				ptrWrite -= _n * size;
			}
			PlayEOF = false;

		}

		public bool PlayEOF {
			get ;
			private set;
		}

		public AudioClip Clip{
			get{ 
				return clip;
			}
		}

		private AudioClip clip ;

		private float[] buffer;
		private int ptrRead; 
		private int ptrWrite;
		private int size;
	}
}