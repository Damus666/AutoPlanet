using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breaker : Building
{
    float activationDistance = 2;
    BoxCollider2D boxcollider;

    Vector3 offset = new Vector3(0.15f,0.3f,0);
    Vector3 leftOffset = new Vector3(-0.15f, 0.3f, 0);

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!Player.i.hasVeichle)
            {
                float dist = Vector3.Distance(transform.position, Player.i.transform.position);
                if (dist <= activationDistance)
                {
                    Player.i.hasVeichle = true;
                    Player.i.veichle = this;
                    boxcollider.enabled = false;
                    Tools.i.SelectTool(-1);
                    Tools.i.gameObject.SetActive(false);
                }
            } else
            {
                if (Player.i.veichle == this && Player.i.CanGoOffVeichle())
                {
                    Player.i.hasVeichle = false;
                    Player.i.veichle = null;
                    boxcollider.enabled = true;
                    Player.i.transform.position += new Vector3(0, 1.5f, 0);
                    Tools.i.gameObject.SetActive(true);
                }
            }
        }
        if (Player.i.hasVeichle && Player.i.veichle == this)
        {
            transform.position = Player.i.transform.localScale.x > 0 ? Player.i.transform.position + offset : Player.i.transform.position + leftOffset;
            if (Mathf.Sign(transform.localScale.x) != Mathf.Sign(Player.i.transform.localScale.x))
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
            }
        }
    }

    public override void FinishInit()
    {
        Player.i = GameObject.Find("Player").GetComponent<Player>();
        boxcollider = GetComponent<BoxCollider2D>();
        CircleCollider2D newcollider = gameObject.AddComponent<CircleCollider2D>();
        newcollider.isTrigger = true;
        newcollider.radius = 0.2f;
        newcollider.offset = new Vector2(0.4f, 0);
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("tile")) return;
        
        if (collision.gameObject.layer == 6)
        {
            ToolInteract.i.DestroyTile(collision.gameObject, true);
        }  else if (collision.gameObject.layer == 7)
        {
            ToolInteract.i.DestroyDecoration(collision.gameObject);
        }
    }
}
