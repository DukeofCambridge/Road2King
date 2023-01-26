using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour
{
    Button NewButton;
    Button ContinueButton;
    Button QuitButton;
    PlayableDirector director;
    private void Awake()
    {
        NewButton = transform.GetChild(1).GetComponent<Button>();
        ContinueButton = transform.GetChild(2).GetComponent<Button>();
        QuitButton = transform.GetChild(3).GetComponent<Button>();
        NewButton.onClick.AddListener(PlayTimeline);
        ContinueButton.onClick.AddListener(Continue);
        QuitButton.onClick.AddListener(QuitGame);

        director = FindObjectOfType<PlayableDirector>();
        director.stopped += NewGame;
    }
    void NewGame(PlayableDirector obj)
    {
        //删除之前的数据
        PlayerPrefs.DeleteAll();
        SceneController.Instance.ToFirstLevel();
    }
    void Continue()
    {
        SceneController.Instance.LoadGame();
    }
    void QuitGame()
    {
        Application.Quit();
    }
    void PlayTimeline()
    {
        director.Play();
    }
}
