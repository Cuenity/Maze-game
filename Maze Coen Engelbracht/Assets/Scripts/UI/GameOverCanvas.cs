using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverCanvas : MonoBehaviour
{
    private TextMeshProUGUI gameOverText;
    private TextMeshProUGUI noEscapeText;
    private Button noButton;
    private UIManager localUIManager;

    private void Awake()
    {
        localUIManager = GameInstance.Instance.uiManager;

        Transform gameOverPanel = transform.Find("GameOverPanel");
        gameOverText = gameOverPanel.Find("GameOverText").GetComponent<TextMeshProUGUI>();
        noEscapeText = gameOverPanel.Find("NoEscapeText").GetComponent<TextMeshProUGUI>();
        noButton = gameOverPanel.Find("NoButton").GetComponent<Button>();
    }

    // sets game over text
    public void GameOver(bool playerWon)
    {
        // since exiting the game is not possible inside the editor:
#if UNITY_EDITOR
        noButton.interactable = false;
        noEscapeText.gameObject.SetActive(true);
#else
        noButton.interactable = true;
        noEscapeText.gameObject.SetActive(false);
#endif

        if (playerWon)
        {
            gameOverText.text = "<size=48> <color=#BC3135>You won! </color> </size>" + Environment.NewLine +
                                Environment.NewLine +
                                "To the victor goes the spoils." + Environment.NewLine +
                                "Enjoy the fruit.";
        }
        else
        {
            gameOverText.text = "Oh no! Your feet got wet!";
        }
    }

    // triggered by the "Yes" button 
    public void YesPressed()
    {
        gameObject.SetActive(false);
        localUIManager.SetCreateMazePanelActive();
    }

    // triggered by the "No" button
    public void NoPressed()
    {
        Application.Quit();
    }
}
