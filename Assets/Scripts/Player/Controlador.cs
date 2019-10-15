using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controlador : MonoBehaviour
{
    private SpriteRenderer spritePlayer;
    private Animator animPlayer;
    private bool pula, noChao;
    private Rigidbody2D rbPlayer;

    [SerializeField] private float velocidade, forcaPulo, detectaChaoR, velocidadeDeslizar, tempoDeslize;
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

        if (movimento != 0 && !animPlayer.GetBool("deslizando"))
        {
            spritePlayer.flipX = (movimento < 0);
        }
        animPlayer.SetBool("andando", movimento != 0);

        //Pulo
        pula = (Input.GetButtonDown("Jump"));
        noChao = Physics2D.OverlapCircle(detectaChao + this.transform.position, detectaChaoR, chao);

        animPlayer.SetBool("pular", !noChao);
        animPlayer.SetFloat("pulo",(rbPlayer.velocity.y < 0)? 1 : 0);

        //Deslize
        if(Input.GetButtonDown("Fire2") && !animPlayer.GetBool("deslizando"))
            StartCoroutine("Deslizar");
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

        //Deslizar
        if (animPlayer.GetBool("deslizando"))
        {
            rbPlayer.velocity = new Vector2(velocidadeDeslizar * ((spritePlayer.flipX)? -1 : 1), rbPlayer.velocity.y);
        }
    }

    IEnumerator Deslizar()
    {
        animPlayer.SetBool("deslizando", true);
        yield return new WaitForSeconds(tempoDeslize);
        animPlayer.SetBool("deslizando", false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectaChao + this.transform.position, detectaChaoR);
    }
}
