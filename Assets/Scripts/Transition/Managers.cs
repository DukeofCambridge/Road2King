using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : Singleton<Managers>
{
    protected override void Awake()
    {
        base.Awake();
        //加载场景时不销毁该对象
        DontDestroyOnLoad(this);
    }
}
