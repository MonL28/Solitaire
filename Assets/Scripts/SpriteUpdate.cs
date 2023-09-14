using System.Collections.Generic;
using UnityEngine;

public class SpriteUpdate : MonoBehaviour
{
    public Sprite cardFace;
    public Sprite cardBack;
    SpriteRenderer spriteRenderer;
    Selectable selectable;
    SolitareScript solitare;
    UserInput userInput;


    private void Start()
    {
        List<string> deck = SolitareScript.GenerateDeck();
        solitare = FindObjectOfType<SolitareScript>();
        userInput = FindObjectOfType<UserInput>();

        int i = 0;

        foreach (string card in deck)
        {
            if (this.name == card)
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
        if (selectable.faceUp == true)
        {
            spriteRenderer.sprite = cardFace;
        }
        else
        {
            spriteRenderer.sprite = cardBack;
        }

        if (userInput.slot1)
        {
            if (name == userInput.slot1.name)
            {
                spriteRenderer.color = Color.yellow;
            }
            else
            {
                spriteRenderer.color = Color.white;
            }
        }
    }

}
