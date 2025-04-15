using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonController : MonoBehaviour
{
    [SerializeField] private string newGameLevel = "Novel";
    public void NewGameButton()
    {
        SceneManager.LoadScene(newGameLevel);
    }
    public void QuitGameButton()
    {
        Application.Quit();
    }
}
