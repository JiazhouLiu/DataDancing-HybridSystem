using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;

public class ReceiveInt : MonoBehaviour
{
    public int shoeReceiver = 9999;
    public int batteryReceiver = 9999;
    // Start is called before the first frame update

    public void GetInt(OSCMessage message)
    {
        shoeReceiver = message.Values[0].IntValue;
        batteryReceiver = message.Values[1].IntValue;
    }
}
