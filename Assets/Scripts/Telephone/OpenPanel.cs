using UnityEngine;
using UnityEngine.UI;

public class Open_Panel : MonoBehaviour
{
    public GameObject panel; // Перетащите сюда вашу панель в инспекторе
    void Start()
    {
        //panel.SetActive(false); // Скрыть панель в начале
    }

    public void TogglePanel() // Метод, который будет вызываться при нажатии на кнопку
    {
        panel.SetActive(!panel.activeSelf);
    }
}
