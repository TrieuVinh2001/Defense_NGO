using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Buff{
    bounceBullet, 
    doubleBullet,
    piercingBullet
}

public class ItemBuff : MonoBehaviour
{
    public Buff buff;
}
