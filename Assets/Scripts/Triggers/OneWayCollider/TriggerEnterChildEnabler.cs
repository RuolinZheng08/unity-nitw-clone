using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnterChildEnabler : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision) {
        ChildEnabler();
    }

    void ChildEnabler() {
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
