using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class CardControllerAuditiva : MonoBehaviour
{
    public int id;
    public bool isFlipped;
    public Image CardImage { get; set; }
    public AudioClip CardSound { get; set; }
    private GameplayControllerAuditiva Controller { get; set; }

    private void Awake()
    {
        Controller = FindFirstObjectByType<GameplayControllerAuditiva>();
        CardImage = gameObject.GetComponentInChildren<Image>();
        isFlipped = false;
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => Controller.FlipCard(this));
    }


    public IEnumerator Flip()
    {
        var rectTransform = GetComponent<RectTransform>();

        if (isFlipped)
        {
            for (var i = 0f; i <= 180; i += 10f)
            {
                rectTransform.rotation = Quaternion.Euler(0, i, 0);
                if (i == 90) CardImage.sprite = Controller.CardBack;
                isFlipped = false;
                yield return new WaitForSeconds(0.01f);
                rectTransform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else
        {
            for (var i = 180f; i >= 0f; i -= 10f)
            {
                rectTransform.rotation = Quaternion.Euler(0, i, 0);
                if (i == 90)
                {
                    CardImage.sprite = Controller.FlippedCard;
                    Controller.AudioSource.PlayOneShot(Controller.CardData[id].cardSound);
                }

                isFlipped = true;
                yield return new WaitForSeconds(0.01f);
                rectTransform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    public void Match()
    {
        Debug.Log("Formou par!");
        CardImage.sprite = Controller.CardData[id].cardSprite;
        Destroy(this.gameObject, 2f);
    }
}