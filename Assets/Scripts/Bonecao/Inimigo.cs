using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inimigo : MonoBehaviour
{
    private Animator anim;
    [SerializeField] GameObject[] drops;

    void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    public void Dano()
    {
        anim.SetTrigger("Apanhou");
        if (Random.Range(0, 100) <= 50)
            Destroy(this.gameObject);
        else
        {
            Instantiate(drops[Random.Range(0,2)], this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
