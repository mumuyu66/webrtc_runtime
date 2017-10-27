using UnityEngine;
using System.Collections;

namespace Module.Audio
{
	public interface IRecord
	{
		int Id { get;}

		void Init (int id, IChatManager chat);

		void StartRecord ();

		void EndRecord ();

		void PushToBuff (float[] source, int vad);

		void Clean ();
	}
}