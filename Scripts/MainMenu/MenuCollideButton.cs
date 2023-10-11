using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuCollideButton : MonoBehaviour
{
    public UnityEvent onClickEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        onClickEvent.Invoke();
    }
}
