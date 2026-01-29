using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions : MonoBehaviour
{
    public static Action<string, sbyte> SendSerial;
    public static Action<GameObject> collision;
    public static Action Begin;

    public static Action<GameObject> Hover;
    public static Action<GameObject> HoverExit;

    public static Action pauseGame;
    public static Action restartGame;
}
