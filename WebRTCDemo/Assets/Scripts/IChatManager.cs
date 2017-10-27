using UnityEngine;
using System.Collections;

public interface IChatManager 
{
	void SendVoidMessage (byte[] data, int len);
		
	void SendFriendMessage (byte[] data, int len,int dataLen);

}

