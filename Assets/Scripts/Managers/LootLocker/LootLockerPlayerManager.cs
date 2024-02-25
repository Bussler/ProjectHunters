using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;

// Start login process for LootLocker
public class LootLockerPlayerManager : MonoBehaviour
{
    public static LootLockerPlayerManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            StartCoroutine(LootLockerLogin());
        }
    }

    IEnumerator LootLockerLogin()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("LootLockerLogin - success: " + response.text);
                PlayerPrefs.SetString("player_identifier", response.player_identifier.ToString());
                done = true;
            }
            else
            {
                Debug.Log("LootLockerLogin - error: " + response.text);
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }
}
