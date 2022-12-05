using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public GameObject flashHolder;
    public Sprite[] flashSprites;
    public SpriteRenderer[] spriteRenderers;

    public float flashtime;

    private void Start()
    {
        Deactivate();
        
    }
    public void Activate()
    {
        flashHolder.SetActive(true);

        int flashSpriteindex=Random.Range(0, flashSprites.Length);
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = flashSprites[flashSpriteindex];
        }

        Invoke("Deactivate", flashtime);
    }

    public void Deactivate()
    {
        flashHolder.SetActive(false);
    }
}
