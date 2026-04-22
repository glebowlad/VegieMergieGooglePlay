using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropClear : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
       collision.gameObject.SetActive(false);
    }
}
