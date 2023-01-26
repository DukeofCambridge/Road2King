using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : Singleton<SceneController>,IEndGameObserver
{
    public GameObject playerPrefab;
    public SceneFader SceneFaderPrefab;
    bool fadeFinished;
    GameObject player;
    NavMeshAgent playerAgent;
    protected override void Awake()
    {
        base.Awake();
        //���س���ʱ�����ٸö���
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        GameManager.Instance.AddObserver(this);
        fadeFinished = true;
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
        SaveManager.Instance.SavePlayerData();
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefab, GetDestination(tag).transform.position, GetDestination(tag).transform.rotation);
            SaveManager.Instance.LoadPlayerData();
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
    //�����ϴα���ĳ���
    public void LoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }
    //�����һ��
    public void ToFirstLevel()
    {
        StartCoroutine(LoadLevel("MainScene"));
    }
    //���ؿ�ʼ��������ĳ���
    IEnumerator LoadLevel(string scene)
    {
        SceneFader fade = Instantiate(SceneFaderPrefab);
        if (scene != "")
        {
            yield return StartCoroutine(fade.fadeOut(1f));
            yield return SceneManager.LoadSceneAsync(scene);
            yield return Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);
            SaveManager.Instance.SavePlayerData();
            yield return StartCoroutine(fade.fadeIn(1f));
            yield break;
        }
    }
    //���ؿ�ʼ����
    public void ToEntryLevel()
    {
        StartCoroutine(LoadEntry());
    }
    IEnumerator LoadEntry()
    {
        yield return SceneManager.LoadSceneAsync("Entry");
        yield break;
    }
    IEnumerator LoadMain()
    {
        SceneFader fade = Instantiate(SceneFaderPrefab);
        yield return StartCoroutine(fade.fadeOut(2f));
        yield return SceneManager.LoadSceneAsync("Entry");
        yield return StartCoroutine(fade.fadeIn(2f));
        yield break;
    }
    public void GetNotified()
    {
        //��Ϊ���������ȫ�ֹ㲥����Update������ֹ���Ͽ�����Э�̵���ϵͳ����
        if (fadeFinished)
        {
            fadeFinished = false;
            StartCoroutine(LoadMain());
        }
        
    }
}
