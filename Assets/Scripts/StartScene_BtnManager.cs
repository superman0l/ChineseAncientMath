using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScene_BtnManager : MonoBehaviour
{
    [SerializeField] Button StartButton;
    [SerializeField] Button ExitButton;

    [SerializeField] string LevelPickScene;
    void Start()
    {
        StartButton.onClick.AddListener(() => GameStart());
        ExitButton.onClick.AddListener(() => GameExit());
    }

    void GameStart()
    {
        SoundManager.Instance.PlayClickSound();
        SceneManager.LoadScene(LevelPickScene);
    }

    void GameExit()
    {
        SoundManager.Instance.PlayClickSound();
        Application.Quit();
    }
}
