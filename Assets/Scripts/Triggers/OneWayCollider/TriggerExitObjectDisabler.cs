using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerExitObjectDisabler : MonoBehaviour
{
    void OnTriggerExit2D(Collider2D collision) {
        this.gameObject.SetActive(false);
    }
}
