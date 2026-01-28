using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions : MonoBehaviour
{
    public static Action<string, sbyte> SendSerial;
    public static Action<GameObject> collision;
    public static Action triggered;
}
