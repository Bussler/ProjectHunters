using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


// Manage scores: get, set, update; implements singleton pattern
public class LeaderBoardManager : MonoBehaviour
{
    public static LeaderBoardManager instance;

    [SerializeField]
    private string leaderboardID = "20442";

    private int score;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        StartCoroutine(LootLockerScoreDownload());
    }

    public void AddScore(int additionalScore)
    {
        this.score += additionalScore;
    }
    
    IEnumerator LootLockerScoreUpload(string playerID)
    {
        bool done = true;

        LootLockerSDKManager.SubmitScore(playerID, this.score, this.leaderboardID, (response) =>
        {
            if (response.statusCode == 200)
            {
                Debug.Log("Successful score upload to LootLocker: ");
            }
            else
            {
                Debug.Log("Failed score upload to LootLocker: " + response.text);
            }
        });

        yield return new WaitWhile(() => done == false);
    }

    IEnumerator LootLockerScoreDownload()
    {
        bool done = false;

        LootLockerSDKManager.GetScoreList(this.leaderboardID, 10, 0, (response) =>
        {
            if (response.statusCode == 200)
            {
                Debug.Log("Successful score download from LootLocker: " + response.text);
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

    IEnumerator LootLockerPlayerScoreDownload(string playerID)
    {
        bool done = false;

        LootLockerSDKManager.GetMemberRank(this.leaderboardID, playerID, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successful score download from LootLocker: " + response.text);
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
