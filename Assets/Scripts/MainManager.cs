using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text NameText;
    public Text ScoreText;
    public Text BestScoreText;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    private string path;
    private BestScore bestScore;
    private string playerName;

    // Start is called before the first frame update
    void Start()
    {
        path = Application.persistentDataPath + "/best_score.json";

        if (GameManager.Instance != null)
            playerName = GameManager.Instance.PlayerName;
        else
            playerName = "default";
        NameText.text = playerName;

        LoadBestScore();

        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = UnityEngine.Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            SaveBestScore();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void LoadBestScore()
    {
        if (!File.Exists(path)) return;

        string jsonScore = File.ReadAllText(path);
        bestScore = JsonUtility.FromJson<BestScore>(jsonScore);
        BestScoreText.text = "Best Score : " 
            + bestScore.PlayerName 
            + " : " 
            + bestScore.Score;
    }

    void SaveBestScore()
    {
        if (bestScore != null)
        {
            if (m_Points <= bestScore.Score) return;
        }
        bestScore = new BestScore(playerName, m_Points);
        
        string jsonScore = JsonUtility.ToJson(bestScore);
        File.WriteAllText(path, jsonScore);
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
    }

    [Serializable]
    class BestScore
    {
        [SerializeField] private string playerName;
        [SerializeField] private int score;

        public BestScore(string playerName, int score)
        {
            this.playerName = playerName;
            this.score = score;
        }

        public string PlayerName { get => playerName; }
        public int Score { get => score; }
    }
}
