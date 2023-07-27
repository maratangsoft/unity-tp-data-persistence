using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUIHandler : MonoBehaviour
{
    [SerializeField]
    private InputField playerNameInput;
    [SerializeField]
    private Button startButton;

    void Start()
    {
        startButton.onClick.AddListener(StartButtonClicked);
    }

    void StartButtonClicked()
    {
        GameManager.Instance.PlayerName = playerNameInput.text;
        SceneManager.LoadScene(1);
    }
}
