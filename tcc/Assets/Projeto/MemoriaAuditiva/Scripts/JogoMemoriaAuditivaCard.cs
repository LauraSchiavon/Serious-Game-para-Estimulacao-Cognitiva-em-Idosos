using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Jogo da Memoria/Memoria auditiva")]
public class JogoMemoriaAuditivaCard : ScriptableObject
{
    public Sprite cardSprite;
    public AudioClip cardSound;
}