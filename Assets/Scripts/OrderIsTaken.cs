using UnityEngine;

public class IntersectionField : MonoBehaviour
{
    public GameObject objectToHide; // Ссылка на GameObject, который нужно скрыть

    void Start()
    {
        // Проверяем, что objectToHide назначен в инспекторе
        if (objectToHide == null)
        {
            Debug.LogError("Object to hide is not assigned! Please assign it in the Inspector.");
            enabled = false; // Отключаем скрипт, чтобы избежать ошибок
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Этот код выполняется, когда другой коллайдер входит в триггер

        // Скрываем GameObject
        objectToHide.SetActive(false);
        Debug.Log("GameObject hidden due to collision with: " + other.gameObject.name);
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    //Этот код выполняется, когда коллайдер выходит из триггера

    //    //Раскрываем GameObject
    //    objectToHide.SetActive(true);
    //    Debug.Log("GameObject unhidden due to exit with: " + other.gameObject.name);
    //}
}
