using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class photoeye : MonoBehaviour
{
    public bool blocked;

    // Detect when an object enters the photoeye beam
    private void OnTriggerEnter(Collider collision)
    {
        blocked = true;
    }

    // Detect when an object exits the photeye beam
    private void OnTriggerExit(Collider collision)
    {
        blocked = false;
    }
}
