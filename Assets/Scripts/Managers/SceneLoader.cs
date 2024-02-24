using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private string MenuScene = "MenuScene";

    [SerializeField]
    private string PlayScene = "SimpleLevel";

    public static SceneLoader instance;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject); // Keep the SceneManager alive between scenes
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadMenuScene()
    {
        LoadScene(MenuScene);
    }

    public void LoadPlayScene()
    {
        LoadScene(PlayScene);
    }

}
