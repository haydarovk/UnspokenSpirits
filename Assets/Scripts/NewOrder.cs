using System.Collections;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public GameObject characterObject; // Ссылка на GameObject персонажа
    public GameObject relatedObject;    // Ссылка на связанный GameObject
    public float time1;
    public float time2;
    public float time3;
    public float time4;

    void Start()
    {
        // Проверка наличия компонентов
        if (characterObject == null)
        {
            Debug.LogError("Character GameObject is not assigned!");
            enabled = false; // Отключаем скрипт, чтобы избежать ошибок
            return;
        }
        if (relatedObject == null)
        {
            Debug.LogError("Related GameObject is not assigned!");
            enabled = false; // Отключаем скрипт, чтобы избежать ошибок
            return;
        }

        // Запускаем корутину, которая будет управлять видимостью персонажа и картинки
        StartCoroutine(CharacterSequence());
    }

    IEnumerator CharacterSequence()
    {
        //yield return Input.GetMouseButtonDown(0);
        // 3 секунды ждем
        yield return new WaitForSeconds(time1);

        // Персонаж становится видимым
        characterObject.SetActive(true);

        // Ждем 1 секунду
        yield return new WaitForSeconds(time2);

        // Активируем связанную картинку
        relatedObject.SetActive(true);

        // Ждем 5 секунд
        yield return new WaitForSeconds(time3);

        // Связанная картинка пропадает
        relatedObject.SetActive(false);

        // Ждем еще 5 секунд
        yield return new WaitForSeconds(time4);

        // Персонаж пропадает
        characterObject.SetActive(false);

        Debug.Log("Sequence complete!");
    }
}
