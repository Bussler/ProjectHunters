using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.SceneManagement;

public class ShowScores : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI[] texts;

    // Start is called before the first frame update
    void Start()
    {
       if(SceneManager.GetActiveScene().name == "MenuScene")
       {
           StartCoroutine(UpdateScoreboard());
       }
    }

    public IEnumerator UpdateScoreboard()
    {
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(LootLockerManager.Instance.LootLockerScoreDownload(10, (response) =>
        {
            if (response != null)
            {
                for (int i = 0; i < response.Length; i++)
                {
                    string playername = string.IsNullOrEmpty(response[i].player.name)? response[i].member_id : response[i].player.name;
                    texts[i].text = response[i].rank + ". " + playername + " Score: " + response[i].score;
                }
            }
        }));
    }
}
