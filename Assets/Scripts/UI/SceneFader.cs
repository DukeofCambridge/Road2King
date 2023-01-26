using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    CanvasGroup cgroup;
    public float fadeinDuration;
    public float fadeoutDuration;
    private void Awake()
    {
        cgroup = gameObject.GetComponent<CanvasGroup>();
        //通用遮罩层
        DontDestroyOnLoad(gameObject);
    }
    public IEnumerator fadeOut(float time)
    {
        while (cgroup.alpha < 1)
        {
            cgroup.alpha += Time.deltaTime / time;
            yield return null;
        }
    }
    public IEnumerator fadeIn(float time)
    {
        while (cgroup.alpha !=0)
        {
            cgroup.alpha -= Time.deltaTime / time;
            yield return null;
        }
        //开启淡入的时候实例化了一个遮罩层，淡出后需要销毁对象
        Destroy(gameObject);
    }
    public IEnumerator fadeOutIn()
    {
        yield return fadeOut(fadeoutDuration);
        yield return fadeIn(fadeinDuration);
    }
}
