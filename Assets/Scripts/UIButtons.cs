using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtons : MonoBehaviour
{
    public GameObject highScorePanel;

    public void PlayAgain()
    {
        highScorePanel.SetActive(false);
        ResetScene();
    }
    public void ResetScene()
    {
        // find all the cards and remove them
        SpriteUpdate[] cards = FindObjectsOfType<SpriteUpdate>();

        foreach(SpriteUpdate card in cards)
        {
            Destroy(card.gameObject);
        }
        //clear the top values
        ClearTopValues();
        //deal new cards
        FindObjectOfType<SolitareScript>().PlayCards();
    }
    void ClearTopValues()
    {
        Selectable[] selectables = FindObjectsOfType<Selectable>();
        foreach(Selectable selectable in selectables)
        {
            if(selectable.CompareTag("Top"))
            {
                selectable.suit = null;
                selectable.value = 0;
            }
        }
    }
}
