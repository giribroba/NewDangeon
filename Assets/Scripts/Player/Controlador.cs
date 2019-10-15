using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controlador : MonoBehaviour
{
    private SpriteRenderer spritePlayer;
    private Animator animPlayer;
    private bool pula, noChao;
    private Rigidbody2D rbPlayer;

    [SerializeField] private float velocidade, forcaPulo, detectaChaoR;
    [SerializeField] private Vector3 detectaChao;
    [SerializeField] private LayerMask chao;
    private float movimento;

    void Start()
    {
        animPlayer = GetComponent<Animator>();
        rbPlayer = GetComponent<Rigidbody2D>();
        spritePlayer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //Flip e input movimento
        movimento = Input.GetAxisRaw("Horizontal");

        if (movimento != 0)
        {
            spritePlayer.flipX = (movimento < 0);
        }
        animPlayer.SetBool("andando", movimento != 0);

        //Pulo
        pula = (Input.GetButtonDown("Jump"));
        noChao = Physics2D.OverlapCircle(detectaChao + this.transform.position, detectaChaoR, chao);

        animPlayer.SetBool("pular", !noChao);
        animPlayer.SetFloat("pulo",(rbPlayer.velocity.y < 0)? 1 : 0);
    }

    private void FixedUpdate()
    {
        //Andar
        rbPlayer.velocity = new Vector2(movimento * velocidade, rbPlayer.velocity.y);

        //Pular
        if (pula && noChao)
        {
            rbPlayer.AddForce(Vector2.up * forcaPulo);
            pula = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectaChao + this.transform.position, detectaChaoR);
    }
}
