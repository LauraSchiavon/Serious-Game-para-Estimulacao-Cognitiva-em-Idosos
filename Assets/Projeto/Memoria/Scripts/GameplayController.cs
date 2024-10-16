using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameplayController : MonoBehaviour
{
    //Imagem Padrão
    [field: Header("Cartas")] [field: SerializeField]
    public Sprite CardBack { get; set; }

    [field: SerializeField] public JogoMemoriaImagemCard[] CardData { get; private set; } // Imagens das cartas

    [field: Space] [field: Header("Tamanho")] [field: SerializeField]
    private int GridSizeX { get; set; }

    [field: SerializeField] private int GridSizeY { get; set; }

    [SerializeField] private GameObject[] slots;
    [field: Space] [field: Header("Sons")] private AudioSource AudioSource { get; set; }
    [SerializeField] private AudioClip SomClick { get; set; }
    [SerializeField] private AudioClip SomUnmatch { get; set; }
    [SerializeField] private AudioClip SomMatch { get; set; }

    [Space] [Header("Components")] [SerializeField]
    private GameObject cardPrefab; // Prefab da carta

    public GameObject toastPrefab;
    public GameObject winPrefab;

    private List<int> _availableIDs; // Lista de IDs disponíveis para as cartas
    [field: SerializeField] private List<CardController> FlippedCards { get; set; } // Lista das cartas viradas
    private List<CardController> MatchedCards { get; set; } // Lista das cartas combinadas

    private void Start()
    {
        Components();
        Shuffle(CardData);
        FlippedCards = new List<CardController>();
        MatchedCards = new List<CardController>();
        InitializeCards();
    }

    private void Components()
    {
        AudioSource = gameObject.GetComponent<AudioSource>();
    }

    private void InitializeCards()
    {
        var cards = GameObject.Find("Cards");
        var totalPairs = (GridSizeX * GridSizeY) / 2;
        _availableIDs = new List<int>();

        for (var i = 0; i < totalPairs; i++)
        {
            _availableIDs.Add(i);
            _availableIDs.Add(i);
        }


        Shuffle(_availableIDs); // Embaralhar a lista de IDs
        for (var x = 0; x < GridSizeX; x++)
        {
            for (var y = 0; y < GridSizeY; y++)
            {
                var index = y * GridSizeX + x; // Índice baseado na posição do grid

                var cardObj = Instantiate(cardPrefab,
                    slots[index].transform.position,
                    Quaternion.identity,
                    slots[index].transform);

                var card = cardObj.GetComponent<CardController>();

                card.id = _availableIDs[index];
            }
        }
    }

    private static void Shuffle<T>(List<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = Random.Range(0, n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    private static void Shuffle<T>(T[] array)
    {
        var n = array.Length;
        for (var i = 0; i < n - 1; i++)
        {
            var j = i + UnityEngine.Random.Range(0, n - i);
            (array[j], array[i]) = (array[i], array[j]);
        }
    }


    public void FlipCard(CardController card)
    {
        if (!card.isFlipped && FlippedCards.Count < 2)
        {
            StartCoroutine(card.Flip());
            SoundPlay(SomClick);
            FlippedCards.Add(card);
            if (FlippedCards.Count == 2)
            {
                StartCoroutine(CheckForMatch());
            }
        }
    }

    private IEnumerator CheckForMatch()
    {
        yield return new WaitForSeconds(1f);

        if (FlippedCards[0].id == FlippedCards[1].id)
        {
            //SoundPlay(SomMatch);

            FlippedCards[0].Match();
            FlippedCards[1].Match();

            MatchedCards.Add(FlippedCards[0]);
            MatchedCards.Add(FlippedCards[1]);

            if (MatchedCards.Count == GridSizeX * GridSizeY)
            {
                StartCoroutine(WinCoroutine());
            }
        }
        else
        {
            FlippedCards[0].StartCoroutine((FlippedCards[0].Flip()));
            FlippedCards[1].StartCoroutine((FlippedCards[1].Flip()));
            //SoundPlay(SomUnmatch);
        }

        FlippedCards.Clear();
    }

    private IEnumerator WinCoroutine()
    {
        yield return new WaitForSeconds(.5f);
        yield return new WaitUntil(() =>
            !FindFirstObjectByType<ToastScript>() && !FindFirstObjectByType<CardController>());
        Debug.Log("ganhou");
        var obj = Instantiate(winPrefab);
        obj.SetActive(true);
    }

    public void SoundPlay(AudioClip audio)
    {
//        AudioSource.PlayOneShot(audio);
    }
}