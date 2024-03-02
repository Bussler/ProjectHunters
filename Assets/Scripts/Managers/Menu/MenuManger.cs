using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManger : MonoBehaviour
{
    [SerializeField]
    private string MenuScene = "MenuScene";
    [SerializeField]
    private string PlayScene = "SimpleLevel";

    [SerializeField]
    private GameObject MainMenu;

    [SerializeField]
    private TMP_InputField PlayerName;

    [SerializeField]
    private GameObject PauseMenu;

    [SerializeField]
    private GameObject Scoreboard;

    [SerializeField]
    private TextMeshProUGUI[] Scoreboardtexts;

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
            OnLoadMenuScene();
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
        if (SceneManager.GetActiveScene().name == MenuScene)
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

    public void OnLoadMenuScene()
    {
        LoadScene(MenuScene);
        MainMenu.SetActive(true);
        Scoreboard.SetActive(true);

        UpdateDisplayedPlayerName();
        StartCoroutine(UpdateScoreboard());
    }

    public void UpdateDisplayedPlayerName()
    {
        PlayerName.text = LootLockerManager.Instance.GetPlayerName();
    }

    public void SetPlayerName(string newName)
    {
        LootLockerManager.Instance.SetPlayerName(PlayerName.text);
        StartCoroutine(UpdateScoreboard());
    }

    public IEnumerator UpdateScoreboard()
    {
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(LootLockerManager.Instance.LootLockerScoreDownload(10, (response) =>
        {
            if (response != null)
            {
                for (int i = 0; i < Scoreboardtexts.Length; i++)
                {
                    if (i >= response.Length)
                    {
                        Scoreboardtexts[i].text = "";
                        continue;
                    }

                    string playername = string.IsNullOrEmpty(response[i].player.name) ? response[i].member_id : response[i].player.name;
                    Scoreboardtexts[i].text = response[i].rank + ". " + playername + " Score: " + response[i].score;
                }
            }
        }));
    }

    public void LoadPlayScene()
    {
        LoadScene(PlayScene);
        MainMenu.SetActive(false);
        Scoreboard.SetActive(false);

    }

    public void Restart()
    {
        ShowPauseMenu();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();

        if(Application.isEditor)
            UnityEditor.EditorApplication.isPlaying = false;
    }
}
