using UnityEngine;

public class ClickableItem : MonoBehaviour
{
    public string itemName; // �������� �������� (��������, "OrangeJuice", "Glass", "Shaker")
    public string itemType; // ��� �������� (��������, "Ingredient", "Tool", "Drink")

    // ���� ����� ���������� Unity, ����� �� ���������� ������� �������� ������
    void OnMouseDown()
    {
        Debug.Log("�������� ��: " + itemName + " (���: " + itemType + ")");
        // ����� �� ����� �������� ����� � GameManager ��� ������ ���������,
        // ������� ���������� ���� ���� � ����������� �� �������� ��������� ����.
        FindObjectOfType<GameManager>().HandleItemClick(this);
    }
}
