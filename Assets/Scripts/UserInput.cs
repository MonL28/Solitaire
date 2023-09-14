using System.Linq;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    public GameObject slot1;
    SolitareScript solitare;
    float timer;
    float doubleClickTime = 0.3f;
    int clickCount = 0;
    private void Start()
    {
        solitare = FindObjectOfType<SolitareScript>();
        slot1 = this.gameObject;
    }
    private void Update()
    {
        if (clickCount == 1)
        {
            timer += Time.deltaTime;
        }
        if (clickCount == 2)
        {
            timer = 0;
            clickCount = 1;
        }
        if (timer > doubleClickTime)
        {
            timer = 0;
            clickCount = 0;
        }
        GetMouseClick();
    }
    void GetMouseClick()
    {
        if (Input.GetMouseButton(0))
        {
            clickCount++;
            Vector3 mousePosition = Camera.main.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                if (hit.collider.CompareTag("Deck"))
                {
                    //clicked deck
                    Deck();
                }
                else if (hit.collider.CompareTag("Card"))
                {
                    //clicked card
                    Card(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Top"))
                {
                    //clicked top
                    Top(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Bottom"))
                {
                    //clicked bottom
                    Bottom(hit.collider.gameObject);
                }
            }
        }
    }
    void Deck()
    {
        //deck click actions
        solitare.DealFromDeck();
        slot1 = this.gameObject;
    }
    void Card(GameObject selected)
    {
        //card click actions
        print("CLicked on Card");

        if (!selected.GetComponent<Selectable>().faceUp)
        {
            if (!Blocked(selected))
            {
                selected.GetComponent<Selectable>().faceUp = true; // flip it over
                slot1 = this.gameObject;
            }
        }
        else if (selected.GetComponent<Selectable>().inDeckPile)
        {
            if (!Blocked(selected))
            {
                if (slot1 == selected) // if the same cards is clicked twice
                {
                    if (DoubleClick())
                    {
                        //attempt auto stack
                        AutoStack(selected);
                    }
                }
                else
                {

                    slot1 = selected;
                }
            }
        }
        if (slot1 == this.gameObject)
        {
            slot1 = selected;
        }

        else if (slot1 != selected)
        {
            if (Stackable(selected))
            {
                //stack it
                Stack(selected);
            }
            else
            {
                //select a new card
                slot1 = selected;
            }
        }
        else if (slot1 == selected)
        {
            if (DoubleClick())
            {
                // attempt auto stack
            }
        }

    }
    void Top(GameObject selected)
    {
        //top click action
        print("Clicked on TOP");
        if (slot1.CompareTag("Card"))
        {
            //if the card is an ACE and the empty slot is top then stack
            if (slot1.GetComponent<Selectable>().value == 1)
            {
                Stack(selected);
            }
        }
    }
    void Bottom(GameObject selected)
    {
        //bottom click action
        //if the card is a king and the empty slot is bottom then stack
        if (slot1.CompareTag("Card"))
        {
            if (slot1.GetComponent<Selectable>().value == 13)
            {
                Stack(selected);
            }
        }
    }
    bool Stackable(GameObject selected)
    {
        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();

        // compare them to see if they stack
        if (!s2.inDeckPile)
        {

            // if in top pile must stack suited ACE to KING
            if (s2.top)
            {
                if (s1.suit == s2.suit || (s1.value == 1 && s2.suit == null))
                {
                    if (s1.value == s2.value + 1)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }

            }
            //if in BOTTOM pile must stack alternate colours King to Ace
            else
            {
                if (s1.value == s2.value - 1)
                {
                    bool card1Red = true;
                    bool card2Red = true;

                    if (s1.suit == "C" || s1.suit == "S")
                    {
                        card1Red = false;
                    }
                    if (s2.suit == "C" || s2.suit == "S")
                    {
                        card2Red = false;
                    }
                    if (card1Red == card2Red)
                    {
                        print("Not stackable!");
                        return false;
                    }
                    else
                    {
                        print("Stackable!");
                        return true;
                    }
                }
            }

        }
        return false;
    }
    void Stack(GameObject selected)
    {
        // if on top of king or empty bottom stack the cards in the place

        //else stack the cards with a negative y offset

        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();
        float yOffset = 0.3f;

        if (s2.top || !s2.top && s1.value == 13)
        {
            yOffset = 0;
        }
        slot1.transform.position = new Vector3(selected.transform.position.x, selected.transform.position.y - yOffset, selected.transform.position.z - 0.01f);
        slot1.transform.parent = selected.transform; // this makes the children move with the parents;

        if (s1.inDeckPile) // removes the cards from the top pile to prevent duplicate cards
        {
            solitare.tripsOnDisplay.Remove(slot1.name);

        }
        else if (s1.top && s2.top && s1.value == 1) // allows movement of cards between top spots
        {
            solitare.topPos[s1.row].GetComponent<Selectable>().value = 0;
            solitare.topPos[s1.row].GetComponent<Selectable>().suit = null;
        }
        else if (s1.top)
        {
            solitare.topPos[s1.row].GetComponent<Selectable>().value = s1.value - 1;
        }
        else
        {
            solitare.bottoms[s1.row].Remove(slot1.name);
        }
        s1.inDeckPile = false; // can´t add cards to the trips pile (always okay)
        s1.row = s2.row;

        if (s2.top)
        {
            solitare.topPos[s1.row].GetComponent<Selectable>().value = s1.value;
            solitare.topPos[s1.row].GetComponent<Selectable>().suit = s1.suit;
            s1.top = true;
        }
        else
        {
            s1.top = false;
        }
        // after completing move reset slo1 to be essentialy null
        slot1 = this.gameObject;
    }
    bool Blocked(GameObject selected)
    {
        Selectable s2 = selected.GetComponent<Selectable>();
        if (s2.inDeckPile == true)
        {
            if (s2.name == solitare.tripsOnDisplay.Last())
            {
                return false;
            }
            else
            {
                print(s2.name + "is blocked by" + solitare.tripsOnDisplay.Last());
                return true;
            }

        }
        else
        {
            if (s2.name == solitare.bottoms[s2.row].Last()) // check if it the bottom
            {
                return false;

            }
            else
            {
                return true;
            }
        }
    }
    bool DoubleClick()
    {
        if (timer < doubleClickTime && clickCount == 2)
        {
            print("Double Click");
            return true;
        }
        else
        {
            return false;
        }
    }
    void AutoStack(GameObject selected)
    {
        for (int i = 0; i < solitare.topPos.Length; i++)
        {
            Selectable stack = solitare.topPos[i].GetComponent<Selectable>();
            if (selected.GetComponent<Selectable>().value == 1)
            {
                if (solitare.topPos[i].GetComponent<Selectable>().value == 0) // top position = empty
                {
                    slot1 = selected;
                    Stack(stack.gameObject); // stack ace up top
                    break;                   // in the first empty position found
                }
            }
            else
            {
                if ((solitare.topPos[i].GetComponent<Selectable>().suit == slot1.GetComponent<Selectable>().suit) && (solitare.topPos[i].GetComponent<Selectable>().value == slot1.GetComponent<Selectable>().value - 1))
                {
                    //if it is the last card (no children)
                    if (HasNoChildren(slot1))
                    {

                        slot1 = selected;
                        //find a top spot that matches conditions for auto stacking if it exists
                        string lastCardName = stack.suit + stack.value.ToString();
                        if (stack.value == 1)
                        {
                            lastCardName = stack.suit + "A";
                        }
                        if (stack.value == 11)
                        {
                            lastCardName = stack.suit + "J";
                        }
                        if (stack.value == 12)
                        {
                            lastCardName = stack.suit + "Q";
                        }
                        if (stack.value == 13)
                        {
                            lastCardName = stack.suit + "K";
                        }
                        GameObject lastCard = GameObject.Find(lastCardName);
                        Stack(lastCard);
                        break;
                    }
                }
            }
        }
    }
    bool HasNoChildren(GameObject card)
    {
        int i = 0;
        foreach (Transform child in card.transform)
        {
            i++;
        }
        if (i == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


}
