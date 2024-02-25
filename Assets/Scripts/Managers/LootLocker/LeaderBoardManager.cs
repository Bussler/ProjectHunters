using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


// Manage scores: get, set, update; implements singleton pattern
public class LeaderBoardManager : MonoBehaviour
{
    public static LeaderBoardManager Instance;

    [SerializeField]
    private string leaderboardID = "20442";

    private int score;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); // Keep the SceneManager alive between scenes
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void AddScore(int additionalScore)
    {
        this.score += additionalScore;
    }
    
    // Upload the score to the leaderboard
    IEnumerator LootLockerScoreUpload(string playerID)
    {
        bool done = true;

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
