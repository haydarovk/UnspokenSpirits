using UnityEngine;
using TMPro; // Нужно импортировать для работы с TextMeshPro

public class Guest : MonoBehaviour
{
    public string orderedDrink; // Название заказанного напитка
    public TextMeshProUGUI orderTextUI; // Ссылка на текстовый объект для отображения заказа

    // Метод для установки заказа
    public void SetOrder(string drinkName)
    {
        orderedDrink = drinkName;
        if (orderTextUI != null)
        {
            orderTextUI.text = orderedDrink; // Отображаем заказ
            orderTextUI.gameObject.SetActive(true); // Показываем текст
        }
    }

    // Метод, который вызывается, когда гость получает напиток
    public bool ServeDrink(string servedDrinkName)
    {
        // Проверяем, совпадает ли поданный напиток с заказом
        if (servedDrinkName == orderedDrink)
        {
            Debug.Log("Гость доволен! Получил " + servedDrinkName);
            // Здесь можно добавить логику для начисления очков и ухода гостя
            return true; // Напиток правильный
        }
        else
        {
            Debug.Log("Гость недоволен! Заказывал " + orderedDrink + ", получил " + servedDrinkName);
            // Здесь можно добавить логику для недовольства гостя и ухода
            return false; // Напиток неправильный
        }
    }

    // Метод для "ухода" гостя
    public void Leave()
    {
        // Можно добавить анимацию ухода
        Destroy(gameObject); // Удаляем объект гостя из сцены
    }
}
