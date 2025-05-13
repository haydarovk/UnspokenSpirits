using UnityEngine;

public class ClickableItem : MonoBehaviour
{
    public string itemName; // Название предмета (например, "OrangeJuice", "Glass", "Shaker")
    public string itemType; // Тип предмета (например, "Ingredient", "Tool", "Drink")

    // Этот метод вызывается Unity, когда по коллайдеру объекта кликнули мышкой
    void OnMouseDown()
    {
        Debug.Log("Кликнули по: " + itemName + " (Тип: " + itemType + ")");
        // Здесь мы будем вызывать метод в GameManager или другом менеджере,
        // который обработает этот клик в зависимости от текущего состояния игры.
        FindObjectOfType<GameManager>().HandleItemClick(this);
    }
}
