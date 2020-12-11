using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Inventario : MonoBehaviour
{
    public static Dictionary<string, int> inv_itens = new Dictionary<string, int>();
    private static Vector2 organizador;
    private bool aberto;
    private static List<GameObject> slots = new List<GameObject>();

    private static GameObject slotStatic, posicaoInicialStatic, inventarioStatic;

    [SerializeField] private GameObject inventario, slot, posicaoInicial;

    private void Start()
    {
        inventarioStatic = inventario;
        slotStatic = slot;
        posicaoInicialStatic = posicaoInicial;
    }

    void Update()
    {
        inventario.gameObject.SetActive(aberto);

        if (Input.GetKeyDown(KeyCode.Escape))
            aberto = !aberto;
    }

    public static void Atualizou()
    {
        organizador = new Vector2(-1, 0);

        if (slots.Count != 0)
        {
            foreach (var i in slots)
            {
                Destroy(i);
            }
        }
        slots.Clear();

        foreach (var i in inv_itens)
        {
            if (organizador.x != 0 && organizador.x % 11 == 0)
                organizador = new Vector2(0, -organizador.x / 11);
            else
                organizador = new Vector2(organizador.x + 1, organizador.y);

            print(organizador);
            var temp = Instantiate(slotStatic, ((Vector2)posicaoInicialStatic.transform.position) + (organizador * 35), Quaternion.identity, inventarioStatic.transform);
            slots.Add(temp);

            temp.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Itens/" + i.Key);
            temp.transform.GetChild(1).GetComponent<Text>().text = i.Value.ToString();

        }
    }

}
