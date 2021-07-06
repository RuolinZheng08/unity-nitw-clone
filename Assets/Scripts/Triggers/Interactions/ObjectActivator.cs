using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectActivator : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision) {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D collision) {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
