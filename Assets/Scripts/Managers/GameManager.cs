using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    public CharacterData playerData;
    private CinemachineFreeLook followCamera;
    //��������ʱ�㲥���й���
    List<IEndGameObserver> enemies = new List<IEndGameObserver>();
    protected override void Awake()
    {
        base.Awake();
        //���س���ʱ�����ٸö���
        DontDestroyOnLoad(this);
    }
    public void RegisterPlayer(CharacterData data)
    {
        playerData = data;
        followCamera = FindObjectOfType<CinemachineFreeLook>();
        if (followCamera != null)
        {
            followCamera.Follow = playerData.transform;
            followCamera.LookAt = playerData.transform;
        }
    }
    public void AddObserver(IEndGameObserver observer)
    {
        enemies.Add(observer);
    }
    public void RemoveObserver(IEndGameObserver observer)
    {
        enemies.Remove(observer);
    }
    public void NotifyObservers()
    {
        foreach(var observer in enemies)
        {
            observer.GetNotified();
        }
    }
}
