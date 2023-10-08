using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breaker : Building
{
    float activationDistance = 2;
    BoxCollider2D boxcollider;

    Player player;
    ToolInteract toolInteract;

    Vector3 offset = new Vector3(0.15f,0.3f,0);
    Vector3 leftOffset = new Vector3(-0.15f, 0.3f, 0);

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!player.hasVeichle)
            {
                float dist = Vector3.Distance(transform.position, player.transform.position);
                if (dist <= activationDistance)
                {
                    player.hasVeichle = true;
                    player.veichle = this;
                    boxcollider.enabled = false;
                    player.tools.SelectTool(-1);
                    player.tools.gameObject.SetActive(false);
                }
            } else
            {
                if (player.veichle == this && player.CanGoOffVeichle())
                {
                    player.hasVeichle = false;
                    player.veichle = null;
                    boxcollider.enabled = true;
                    player.transform.position += new Vector3(0, 1.5f, 0);
                    player.tools.gameObject.SetActive(true);
                }
            }
        }
        if (player.hasVeichle && player.veichle == this)
        {
            transform.position = player.transform.localScale.x > 0 ? player.transform.position + offset : player.transform.position + leftOffset;
            if (Mathf.Sign(transform.localScale.x) != Mathf.Sign(player.transform.localScale.x))
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
            }
        }
    }

    public override void FinishInit()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        toolInteract = player.toolInteract;
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
            toolInteract.DestroyTile(collision.gameObject);
        }  else if (collision.gameObject.layer == 7)
        {
            toolInteract.DestroyDecoration(collision.gameObject);
        }
    }
}
