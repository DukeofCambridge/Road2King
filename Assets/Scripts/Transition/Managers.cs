using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : Singleton<Managers>
{
    protected override void Awake()
    {
        base.Awake();
        //���س���ʱ�����ٸö���
        DontDestroyOnLoad(this);
    }
}
