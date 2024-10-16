using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToastScript : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text texto;
    void Awake()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;
        texto = GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
        Destroy(this.gameObject, 10f);
    }

    public void setText(string textoCarta)
    {
        texto.text = textoCarta;
    }

    public void closeToast()
    {
        foreach (GameObject toast in GameObject.FindGameObjectsWithTag("Toast"))
        {
            if (toast != null)
            {
                Destroy(toast);
            }
            else return;
        }
    }
}
