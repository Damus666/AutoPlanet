using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager i;

    public AudioSource hitSound;
    public AudioSource shootSound;
    public AudioSource jetpackSound;
    public AudioSource bgMusic;
    public AudioSource clickSound;
    public AudioSource gameOverSound;

    private void Awake()
    {
        i = this;
    }
}
