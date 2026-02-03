using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void ChooseSide(string playerSide)
    {
        PlayerPrefs.SetString("PlayerSide", playerSide);
        SceneManager.LoadScene("GameScene");
    }
}