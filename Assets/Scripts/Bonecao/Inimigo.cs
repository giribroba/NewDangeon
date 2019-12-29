using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inimigo : MonoBehaviour
{
    Animator anim;
    void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    public void Dano()
    {
        anim.SetTrigger("Apanhou");
    }
}
