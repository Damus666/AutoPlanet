using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuCollideButton : MonoBehaviour
{
    public UnityEvent onClickEvent;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        onClickEvent.Invoke();
    }
}
