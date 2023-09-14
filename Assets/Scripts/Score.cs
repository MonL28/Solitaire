using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    public Selectable[] topStacks;
    public GameObject highScorePanel;
    private void Update()
    {
        if(HasWon())
        {
            Win();
        }
    }
    public bool HasWon()
    {
        int i = 0;

        foreach(Selectable topStack in topStacks)
        {
            i += topStack.value;
        }
        if (i > 52)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void Win()
    {
        highScorePanel.SetActive(true);
        print("You have WON!");
    }
}
