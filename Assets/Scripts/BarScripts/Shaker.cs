using UnityEngine;

public class Shaker : MonoBehaviour
{
    private Animator anim;
    private bool isAtWorkstation;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnMouseDown()
    {
        isAtWorkstation = !isAtWorkstation;
        anim.SetBool("Move", isAtWorkstation);


    }
    public void OnCocktailComplete()
    {
        anim.SetBool("Move", false);
    }
}