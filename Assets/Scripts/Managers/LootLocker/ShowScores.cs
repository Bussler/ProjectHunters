using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

public class ShowScores : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI[] texts;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateScoreboard());
    }

    IEnumerator UpdateScoreboard()
    {
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(LeaderBoardManager.instance.LootLockerScoreDownload(10, (response) =>
        {
            if (response != null)
            {
                for (int i = 0; i < response.Length; i++)
                {
                    texts[i].text = response[i].rank + ". " + response[i].member_id + " Score: " + response[i].score;
                }
            }
        }));
    }
}
