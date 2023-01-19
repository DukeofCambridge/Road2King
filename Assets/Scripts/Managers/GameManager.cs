using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CharacterData playerData;
    //��������ʱ�㲥���й���
    List<IEndGameObserver> enemies = new List<IEndGameObserver>();
    public void RegisterPlayer(CharacterData data)
    {
        playerData = data;
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
