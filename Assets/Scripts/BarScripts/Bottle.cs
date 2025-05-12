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

        // Дополнительные эффекты
        if (isSelected)
        {
            // Подсветка
            GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", Color.yellow);
            // Звук наливания
        }
    }
}