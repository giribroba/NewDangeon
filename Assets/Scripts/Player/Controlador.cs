using System.Collections;
using UnityEngine;

public class Controlador : MonoBehaviour
{
    private int cliques;
    private SpriteRenderer spritePlayer;
    private Animator animPlayer;
    private Collider2D[] atingidos;
    private float velocidadeV, cdAtaquePercorrido, tempoParticulaPercorrido;
    private bool pula, noChao, puloDuplo, atacando;
    private Rigidbody2D rbPlayer;

    [Header("Movimentação")]
    [SerializeField] GameObject particula;
    [SerializeField] private bool podePuloDuplo;
    [SerializeField] private float velocidade, forcaPulo, detectaChaoR, velocidadeDeslizar, tempoDeslize, tempoParticula;
    [SerializeField] private LayerMask chaoL, inimigosL;
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
        //Controle velocidade
        if (animPlayer.GetBool("abaixado") || atacando)
            velocidadeV = 0;
        else
            velocidadeV = velocidade;

        //Controle particula
        tempoParticulaPercorrido += Time.deltaTime;
        if(movimento != 0)
            particula.transform.eulerAngles = new Vector3(0 , (movimento < 0 ) ? 90 : -90, 0);

        if(velocidadeV == 0 || movimento == 0 || !noChao)
            tempoParticulaPercorrido = 0;

        if (tempoParticulaPercorrido < tempoParticula && noChao)
            particula.GetComponent<ParticleSystem>().enableEmission = movimento != 0;

        else
            particula.GetComponent<ParticleSystem>().enableEmission = false;

        //Flip e input movimento
        movimento = Input.GetAxisRaw("Horizontal");

        if (movimento != 0 && !animPlayer.GetBool("deslizando") && !atacando)
            spritePlayer.flipX = (movimento < 0);
        animPlayer.SetBool("andando", movimento != 0);

        //Pulo
        pula = (Input.GetButtonDown("Jump") && !animPlayer.GetBool("abaixado"));
        noChao = Physics2D.OverlapCircle(detectaChao + new Vector2(this.transform.position.x, this.transform.position.y), detectaChaoR, chaoL);
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


        //Input ataque
        cdAtaquePercorrido -= Time.deltaTime;
        if (cdAtaquePercorrido < -0.2f)
            animPlayer.SetFloat("ataque",0);
        if (Input.GetButtonDown("Fire1") && cliques < 3)
            cliques++;
 
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

        //Ataque
        if (cliques > 0 && !atacando && cdAtaquePercorrido < 0)
        {
            cdAtaquePercorrido = cdAtaque;
            atacando = true;
            cliques--;
            animPlayer.SetTrigger("atacando");
            animPlayer.SetFloat("ataque", animPlayer.GetFloat("ataque") + (animPlayer.GetFloat("ataque") == 1 ? -animPlayer.GetFloat("ataque") : 0.5f));
        }
    }

    private void Ataque()
    {
        atingidos = Physics2D.OverlapCircleAll((detectaInimigos - Vector2.right * (detectaInimigos *((GetComponent<SpriteRenderer>().flipX)?2 : 0))) + new Vector2(this.transform.position.x, this.transform.position.y), detectaInimigosR, inimigosL);
        for (int i = 0; i < atingidos.Length; i++)
        {
            atingidos[i].gameObject.GetComponent<Inimigo>().Dano();
        }
    }

    private void TerminouAtaque()
    {
        atacando = false;
    }

    IEnumerator Deslizar()
    {
        animPlayer.SetBool("deslizando", true);
        yield return new WaitForSeconds(tempoDeslize);
        animPlayer.SetBool("deslizando", false);
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
