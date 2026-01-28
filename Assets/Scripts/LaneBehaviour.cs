using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneBehaviour : MonoBehaviour
{
    public Lane lane;
    public byte number;
    public enum Lane
    {
        Player,
        Entity
    }
}
