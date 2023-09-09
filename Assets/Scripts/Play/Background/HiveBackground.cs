using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveBackground : MonoBehaviour
{
    public Collider2D kHiveCol;
    public SpriteRenderer kSprite;
    void Start()
    {
        
    }
    void Update()
    {
        if(Mng.play.IsWithinCollider(kHiveCol, Input.mousePosition))
        {
            
            kSprite.enabled = false;
        }
        else
        {
            print("outside");
            kSprite.enabled = true;
        }
    }
}
