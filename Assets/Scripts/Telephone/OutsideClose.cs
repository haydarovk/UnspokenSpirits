using UnityEngine;
using UnityEngine.EventSystems; // Обязательно для работы с UI Event System
using UnityEngine.UI;

public class ClosePanelOnClickOutside : MonoBehaviour, IPointerClickHandler
{
    public GameObject panel; // Ссылка на панель, которую нужно закрывать

    public void OnPointerClick(PointerEventData eventData)
    {
        // Проверяем, был ли клик внутри панели
        RectTransform panelRectTransform = panel.GetComponent<RectTransform>();
        if (panelRectTransform != null && RectTransformUtility.RectangleContainsScreenPoint(panelRectTransform, eventData.position, eventData.enterEventCamera))
        {
            // Клик был внутри панели, ничего не делаем
            return;
        }

        // Клик был за пределами панели, закрываем панель
        panel.SetActive(false);
    }
}
