using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    public CharacterData playerData;
    private CinemachineFreeLook followCamera;
    //主角死亡时广播所有怪物
    List<IEndGameObserver> enemies = new List<IEndGameObserver>();
    protected override void Awake()
    {
        base.Awake();
        //加载场景时不销毁该对象
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
