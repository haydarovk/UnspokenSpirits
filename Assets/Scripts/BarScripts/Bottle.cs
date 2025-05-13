using UnityEngine;

public class Bottle : MonoBehaviour
{
    private Animator anim;
    private bool isSelected;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnMouseDown()
    {
        isSelected = !isSelected;
        anim.SetTrigger("Pour");

        // �������������� �������
        if (isSelected)
        {
            // ���������
            GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", Color.yellow);
            // ���� ���������
        }
    }
}