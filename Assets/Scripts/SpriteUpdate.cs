using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteUpdate : MonoBehaviour
{
    public Sprite cardFace;
    public Sprite cardBack;
    SpriteRenderer spriteRenderer;
    Selectable selectable;
    SolitareScript solitare;


    private void Start()
    {
        List<string> deck = SolitareScript.GenerateDeck();
        solitare = FindObjectOfType<SolitareScript>();

        int i = 0;

        foreach(string card in deck)
        {
            if(this.name == card)
            {
                cardFace = solitare.cardFaces[i];
                break;
            }
            i++;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        selectable = GetComponent<Selectable>();
    }
    private void Update()
    {
        if(selectable.faceUp == true)
        {
            spriteRenderer.sprite = cardFace;
        }
        else
        {
            spriteRenderer.sprite = cardBack;
        }
    }

}
