using System.Collections;
using UnityEngine;

public class Controlador : MonoBehaviour
{
    private SpriteRenderer spritePlayer;
    private Animator animPlayer;
    private float velocidadeV, cdAtaquePercorrido;
    private bool pula, noChao, puloDuplo, atacando;
    private Rigidbody2D rbPlayer;

    [Header("Movimentação")]
    [SerializeField] private bool podePuloDuplo;
    [SerializeField] private float velocidade, forcaPulo, detectaChaoR, velocidadeDeslizar, tempoDeslize;
    [SerializeField] private LayerMask chao;
    [SerializeField] private Vector2 detectaChao;

    [Header("Ataque")]
    [SerializeField] private Vector2 detectaInimigos;
    [SerializeField] private float cdAtaque, detectaInimigosR;

    [Header("Colliders")]
    [SerializeField] Vector2 colliderNorm;
    [SerializeField] Vector2 colliderPeq, colliderNormPos, colliderPeqPos;
    private float movimento;

    void Start()
    {
        animPlayer = GetComponent<Animator>();
        rbPlayer = GetComponent<Rigidbody2D>();
        spritePlayer = GetComponent<SpriteRenderer>();
        velocidadeV = velocidade;
    }

    void Update()
    {
        //Flip e input movimento
        movimento = Input.GetAxisRaw("Horizontal");

        if (movimento != 0 && !animPlayer.GetBool("deslizando"))
            spritePlayer.flipX = (movimento < 0);
        animPlayer.SetBool("andando", movimento != 0);

        //Pulo
        pula = (Input.GetButtonDown("Jump") && !animPlayer.GetBool("abaixado"));
        noChao = Physics2D.OverlapCircle(detectaChao + new Vector2(this.transform.position.x, this.transform.position.y), detectaChaoR, chao);
        puloDuplo = ((noChao) ? podePuloDuplo : puloDuplo);

        animPlayer.SetBool("pular", !noChao && !(Input.GetButtonDown("Jump") && puloDuplo));
        animPlayer.SetFloat("pulo", (rbPlayer.velocity.y < 0) ? 1 : 0);

        //Deslize
        if (Input.GetButtonDown("Fire2") && !animPlayer.GetBool("deslizando"))
            StartCoroutine("Deslizar");

        //Abaixar
        animPlayer.SetBool("abaixado", (Input.GetAxisRaw("Vertical") < 0));
        this.GetComponent<BoxCollider2D>().size = (animPlayer.GetBool("abaixado") ? colliderPeq : colliderNorm);
        this.GetComponent<BoxCollider2D>().offset = (animPlayer.GetBool("abaixado") ? colliderPeqPos : colliderNormPos);

        //Ataque
        cdAtaquePercorrido -= Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && (cdAtaquePercorrido <= 0 || animPlayer.GetFloat("ataque") > 0))
        {
            animPlayer.SetTrigger("atacando");
            animPlayer.SetFloat("ataque", animPlayer.GetFloat("ataque") + (animPlayer.GetFloat("ataque") >= 1 ? -animPlayer.GetFloat("ataque") : 0.5f));
        }
    }

    private void FixedUpdate()
    {
        //Andar
        rbPlayer.velocity = new Vector2(movimento * velocidadeV, rbPlayer.velocity.y);

        //Pular
        if (pula && (puloDuplo || noChao))
        {
            puloDuplo = noChao;
            rbPlayer.velocity = Vector2.right * rbPlayer.velocity;
            rbPlayer.AddForce(Vector2.up * forcaPulo);
            pula = false;
        }

        //Deslizar
        if (animPlayer.GetBool("deslizando"))
        {
            rbPlayer.velocity = new Vector2(velocidadeDeslizar * ((spritePlayer.flipX) ? -1 : 1), rbPlayer.velocity.y);
        }
    }

    IEnumerator Deslizar()
    {
        animPlayer.SetBool("deslizando", true);
        yield return new WaitForSeconds(tempoDeslize);
        animPlayer.SetBool("deslizando", false);
    }

    IEnumerator Ataque(float tempo)
    {
        velocidadeV = 0;
        atacando = true;
        yield return new WaitForSeconds(tempo);
        atacando = false;
        velocidadeV = velocidade;
    }

    private void OnDrawGizmos()
    {
        //Pulo
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(detectaChao + new Vector2(this.transform.position.x, this.transform.position.y), detectaChaoR);

        //Ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((detectaInimigos - Vector2.right * (detectaInimigos *((GetComponent<SpriteRenderer>().flipX)?2 : 0))) + new Vector2(this.transform.position.x, this.transform.position.y), detectaInimigosR);
    }
}
