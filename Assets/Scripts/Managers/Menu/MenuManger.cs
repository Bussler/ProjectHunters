using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManger : MonoBehaviour
{
    [SerializeField]
    private string MenuScene = "MenuScene";
    [SerializeField]
    private string PlayScene = "SimpleLevel";

    [SerializeField]
    private GameObject MainMenu;

    [SerializeField]
    private GameObject PauseMenu;

    [SerializeField]
    private GameObject Scoreboard;

    private MainControls input = null; // Input system


    public static MenuManger Instance;

    private void Awake()
    {
        input = new MainControls();
    }

    private void OnEnable()
    {
        input.Enable();
        input.menu.pause.performed += HandlePauseMenu;
    }

    private void OnDisable()
    {
        input.Disable();
        input.menu.pause.performed -= HandlePauseMenu;
    }

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (SceneManager.GetActiveScene().name == MenuScene)
        {
            MainMenu.SetActive(true);
            Scoreboard.SetActive(true);
        }
        else
        {
            MainMenu.SetActive(false);
            Scoreboard.SetActive(false);
        }
    }

    private void HandlePauseMenu(InputAction.CallbackContext value)
    {
        if(value.performed)
            ShowPauseMenu();
    }

    public void ShowPauseMenu()
    {
        if(SceneManager.GetActiveScene().name == MenuScene)
            return;

        if (PauseMenu.activeInHierarchy)
        {
            PauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            PauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadMenuScene()
    {
        LoadScene(MenuScene);
        MainMenu.SetActive(true);
        Scoreboard.SetActive(true);
    }

    public void LoadPlayScene()
    {
        LoadScene(PlayScene);
        MainMenu.SetActive(false);
        Scoreboard.SetActive(false);

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
