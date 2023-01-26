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
        //ͨ�����ֲ�
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
        //���������ʱ��ʵ������һ�����ֲ㣬��������Ҫ���ٶ���
        Destroy(gameObject);
    }
    public IEnumerator fadeOutIn()
    {
        yield return fadeOut(fadeoutDuration);
        yield return fadeIn(fadeinDuration);
    }
}
