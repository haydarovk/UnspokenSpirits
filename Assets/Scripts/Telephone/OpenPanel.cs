using UnityEngine;
using UnityEngine.UI;

public class PanelOpener : MonoBehaviour
{
    public GameObject panel; // Перетащите сюда вашу панель в инспекторе
    void Start()
    {
        panel.SetActive(false); // Скрыть панель в начале
    }

    public void OpenPanel() // Метод, который будет вызываться при нажатии на кнопку
    {
        panel.SetActive(true);
    }
}
