using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;


// Manage scores: get, set, update; implements singleton pattern
public class LootLockerManager : MonoBehaviour
{
    public static LootLockerManager Instance;

    [SerializeField]
    private string leaderboardID = "20442";

    [SerializeField]
    private int score;

    void Awake()
    {
        if (!LootLockerSDKManager.CheckInitialized())
            StartCoroutine(LootLockerGuestLogin());
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    IEnumerator LootLockerGuestLogin()
    {
        bool done = false;

        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("LootLockerLogin - success: " + response.text);
                PlayerPrefs.SetString("player_identifier", response.player_id.ToString());
            }
            else
            {
                Debug.Log("LootLockerLogin - error: " + response.text);
            }
            done = true;
        });
        yield return new WaitWhile(() => done == false);
    }

    IEnumerator LootLockerWhiteLabelSignUp(string email, string password)
    {
        bool done = false;

        LootLockerSDKManager.WhiteLabelSignUp(email, password, response =>
        {
            if (!response.success)
            {
                Debug.Log("LootLockerLogin - error: " + response.text);
            }
            else
            {
                Debug.Log("LootLockerLogin - success: " + response.text);
            }
            done = true;
        });

        yield return new WaitWhile(() => done == false);
    }

    IEnumerator LootLockerWhiteLabelLogin(string email, string password, string playername="")
    {
        bool done = false;

        LootLockerSDKManager.WhiteLabelLoginAndStartSession(email, password, true, response =>
        {
            if (response.success)
            {
                Debug.Log("LootLockerLogin - success: " + response.text);
                PlayerPrefs.SetString("player_identifier", response.SessionResponse.player_id.ToString());
                
                StartCoroutine(LookUpPlayername((lootLockerPlayerName) =>
                {
                    if (string.IsNullOrEmpty(lootLockerPlayerName))
                    {
                        LootLockerSDKManager.SetPlayerName(playername, (response) =>
                        {
                            PlayerPrefs.SetString("player_name", playername);
                            done = true;
                        });
                    }
                    else
                    {
                        PlayerPrefs.SetString("player_name", lootLockerPlayerName);
                        done = true;
                    }
                }));
            }
            else
            {
                Debug.Log("LootLockerLogin - error: " + response.text);
                done = true;
            }
        });

        yield return new WaitWhile(() => done == false);

        MenuManger.Instance.UpdateDisplayedPlayerName();
    }

    public IEnumerator LookUpPlayername(System.Action<string> playername)
    {
        bool done = false;

        LootLockerSDKManager.GetPlayerName((response) =>
        {
            if (response.success)
            {
                playername(response.name);
            }
            else
            {
                playername("");
            }
            done = true;
        });

        yield return new WaitWhile(() => done == false);
    }

    public string GetPlayerName()
    {
        return "Player: " + (string.IsNullOrEmpty(PlayerPrefs.GetString("player_name")) ?
            PlayerPrefs.GetString("player_identifier") : PlayerPrefs.GetString("player_name"));
    }

    public void AddScore(int additionalScore)
    {
        this.score += additionalScore;
    }
    
    // Upload the score to the leaderboard
    IEnumerator LootLockerScoreUpload(string playerID)
    {
        bool done = true;
        yield return new WaitWhile(() => LootLockerSDKManager.CheckInitialized() == false);

        LootLockerSDKManager.SubmitScore(playerID, this.score, this.leaderboardID, (response) =>
        {
            if (response.statusCode == 200)
            {
                Debug.Log("Successful score upload to LootLocker: " + this.score);
            }
            else
            {
                Debug.Log("Failed score upload to LootLocker: " + response.text);
            }
        });

        yield return new WaitWhile(() => done == false);
    }

    // Download the top scoreAmount scores from the leaderboard
    public IEnumerator LootLockerScoreDownload(int scoreAmount, System.Action<LootLockerLeaderboardMember[]> callback)
    {
        bool done = false;
        yield return new WaitWhile(() => LootLockerSDKManager.CheckInitialized() == false);

        LootLockerSDKManager.GetScoreList(this.leaderboardID, scoreAmount, 0, (response) =>
        {
            if (response.statusCode == 200)
            {
                Debug.Log("Successful score download from LootLocker: " + response.text);

                LootLockerLeaderboardMember[] scores = response.items;
                callback(scores);

                done = true;
            }
            else
            {
                Debug.Log("Failed score download from LootLocker: " + response.text);
                done = true;
            }
        });

        yield return new WaitWhile(() => done == false);
    }

    // Download the score of a specific player
    public IEnumerator LootLockerPlayerScoreDownload(string playerID, System.Action<int> callback)
    {
        bool done = false;
        yield return new WaitWhile(() => LootLockerSDKManager.CheckInitialized() == false);

        LootLockerSDKManager.GetMemberRank(this.leaderboardID, playerID, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successful score download from LootLocker: " + response.text);
                callback(response.score);
                done = true;
            }
            else
            {
                Debug.Log("Failed score download from LootLocker: " + response.text);
                done = true;
            }
        });

        yield return new WaitWhile(() => done == false);
    }

    public void UploadScore(string playerID = "")
    {
        if (playerID == "")
        {
            playerID = PlayerPrefs.GetString("player_identifier");
        }

        StartCoroutine(LootLockerScoreUpload(playerID));
    }

    public void OnApplicationQuit()
    {
        UploadScore();
    }

}
