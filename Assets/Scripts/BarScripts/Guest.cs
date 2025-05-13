using UnityEngine;
using TMPro; // ����� ������������� ��� ������ � TextMeshPro

public class Guest : MonoBehaviour
{
    public string orderedDrink; // �������� ����������� �������
    public TextMeshProUGUI orderTextUI; // ������ �� ��������� ������ ��� ����������� ������

    // ����� ��� ��������� ������
    public void SetOrder(string drinkName)
    {
        orderedDrink = drinkName;
        if (orderTextUI != null)
        {
            orderTextUI.text = orderedDrink; // ���������� �����
            orderTextUI.gameObject.SetActive(true); // ���������� �����
        }
    }

    // �����, ������� ����������, ����� ����� �������� �������
    public bool ServeDrink(string servedDrinkName)
    {
        // ���������, ��������� �� �������� ������� � �������
        if (servedDrinkName == orderedDrink)
        {
            Debug.Log("����� �������! ������� " + servedDrinkName);
            // ����� ����� �������� ������ ��� ���������� ����� � ����� �����
            return true; // ������� ����������
        }
        else
        {
            Debug.Log("����� ���������! ��������� " + orderedDrink + ", ������� " + servedDrinkName);
            // ����� ����� �������� ������ ��� ������������ ����� � �����
            return false; // ������� ������������
        }
    }

    // ����� ��� "�����" �����
    public void Leave()
    {
        // ����� �������� �������� �����
        Destroy(gameObject); // ������� ������ ����� �� �����
    }
}
