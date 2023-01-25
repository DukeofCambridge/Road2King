using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : Singleton<SceneController>
{
    public GameObject playerPrefab;
    GameObject player;
    NavMeshAgent playerAgent;
    protected override void Awake()
    {
        base.Awake();
        //加载场景时不销毁该对象
        DontDestroyOnLoad(this);
    }
    public void Trans2Destination(TransitionPoint p)
    {
        switch (p.type)
        {
            case TransitionPoint.TransitionType.WithinScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, p.tag));
                break;
            case TransitionPoint.TransitionType.TransScene:
                StartCoroutine(Transition(p.sceneName, p.tag));
                break;
        }
    }
    IEnumerator Transition(string sceneName,TransitionDestination.DestinationTag tag) 
    {
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefab, GetDestination(tag).transform.position, GetDestination(tag).transform.rotation);
            yield break;
        }
        else
        {
            player = GameManager.Instance.playerData.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;
            player.transform.SetPositionAndRotation(GetDestination(tag).transform.position, GetDestination(tag).transform.rotation);
            
            playerAgent.enabled = true;
            yield return null;
        }
    }
    private TransitionDestination GetDestination(TransitionDestination.DestinationTag tag)
    {
        var entrance = FindObjectsOfType<TransitionDestination>();
        foreach(TransitionDestination des in entrance){
            if (des.tag == tag)
            {
                return des;
            }
        }
        return null;
    }
}
