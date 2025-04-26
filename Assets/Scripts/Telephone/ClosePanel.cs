using UnityEngine;

public class PanelCloser : MonoBehaviour
{
    private GameObject panel; // Ссылка на панель

    void Start()
    {
        panel = transform.parent.gameObject; // Получаем панель, в которой находится этот объект

        // Убедитесь, что панель существует
        if (panel == null)
        {
            Debug.LogError("Panel not found!");
        }
    }

    public void ClosePanel()
    {
        // Скрываем панель
        if (panel != null)
        {
            panel.SetActive(false); // Делаем панель неактивной
        }
    }
}
