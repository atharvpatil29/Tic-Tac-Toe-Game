using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public TextMeshProUGUI resultText;

    void Start()
    {
        string result = PlayerPrefs.GetString("Result", "Draw");
        string player = PlayerPrefs.GetString("PlayerSide", "X");

        if (result == "Draw")
            resultText.text = "It's a Draw!";
        else if (result == player)
            resultText.text = "You Win!";
        else
            resultText.text = "You Lose!";
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
