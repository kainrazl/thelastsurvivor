using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScorePlayersList : MonoBehaviour
{
    public GameObject scorePrefab;
    public int maxPlayersList;

    private List<PlayerScoreEntry> highScores = new List<PlayerScoreEntry>();
    private string scoresJSON = "";

    void Start()
    {

        GetMaxScore();

        int scoreCount = 0;

        try
        {
            scoreCount = highScores.Count;
        }
        catch (System.Exception e)
        {
            Debug.Log("ERROR: " + e.Message);
        }

        if (scoreCount > maxPlayersList)
        {
            for (int element = scoreCount - 1; element > maxPlayersList; element--)
            {
                highScores.RemoveAt(element);
            }
        }

        SortScores();

        foreach (PlayerScoreEntry itemScore in highScores)
        {
            GameObject objectScore = Instantiate(scorePrefab, gameObject.transform.position, Quaternion.identity, gameObject.transform);

            foreach (TextMeshProUGUI scoreData in objectScore.GetComponentsInChildren<TextMeshProUGUI>())
            {
                if (scoreData.name.Equals("PlayerName"))
                {
                    scoreData.text = itemScore.name;
                }
                else
                {
                    scoreData.text = itemScore.score.ToString().PadLeft(6, '0');
                }
            }

            scoreCount++;

            if (scoreCount == maxPlayersList)
            {
                break;
            }
        }
    }

    public void SetMaxScore(int score, string name)
    {
        GetMaxScore();

        PlayerScoreEntry newScore = new PlayerScoreEntry { score = score, name = name };
        highScores.Add(newScore);

        scoresJSON = JsonUtility.ToJson(highScores);

        PlayerPrefs.SetString("LocalHighScores", scoresJSON);
        PlayerPrefs.Save();
    }

    public void GetMaxScore()
    {
        //scoresJSON = PlayerPrefs.GetString("LocalHighScores");
        
        if (scoresJSON.Length == 0)
        {
            Debug.Log("Empty json");
            if (highScores.Count <= maxPlayersList)
            {
                for (int element = highScores.Count; element < maxPlayersList; element++)
                {
                    int randScore = UnityEngine.Random.Range(50, 300);
                    PlayerScoreEntry newScore = new PlayerScoreEntry { score = randScore, name = "COM" + element };
                    highScores.Add(newScore);
                }
            }

            scoresJSON = JsonUtility.ToJson(highScores);
        }
        else
        {
            Debug.Log("Json data");
            Debug.Log(scoresJSON);
            highScores = JsonUtility.FromJson<List<PlayerScoreEntry>>(scoresJSON);
        }
    }

    private void SortScores()
    {
        int totalScores = highScores.Count;

        for (int i = 0; i < totalScores; i++)
        {
            for (int j = i + 1; j < totalScores; j++)
            {
                if (highScores[j].score > highScores[i].score)
                {
                    PlayerScoreEntry auxScore = highScores[i];

                    highScores[i] = highScores[j];
                    highScores[j] = auxScore;
                }
            }
        }

        scoresJSON = JsonUtility.ToJson(highScores[0]);
        Debug.Log("Sort: " + scoresJSON.ToString());

        PlayerPrefs.SetString("LocalHighScores", scoresJSON);
        PlayerPrefs.Save();
    }
}

[Serializable]
public class PlayerScoreEntry
{
    public int score { get; set; }
    public string name { get; set; }
}

public class MaxScoreList{
    public List<PlayerScoreEntry> highScores;
}