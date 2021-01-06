using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileScript : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isAlive = false;
    public Sprite deadSprite, aliveSprite;

    void Start()
    {
        
    }

    public bool ChangeState()
    {
        isAlive = !isAlive;
        if (isAlive)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = aliveSprite;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = deadSprite;
        }
        return isAlive;
    }

    public void toAlive()
    {
        isAlive = true;
        gameObject.GetComponent<SpriteRenderer>().sprite = aliveSprite;
    }

    public void toDead()
    {
        isAlive = false;
        gameObject.GetComponent<SpriteRenderer>().sprite = deadSprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
