using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class NPC : MonoBehaviour
{
    private bool _using = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var input = other.GetComponent<StarterAssetsInputs>();

            if (input.use && _using == false)
            {
                Use();
            }
        }
    }

    private void Use()
    {
        _using = true;
        Debug.Log("hui");
        _using = false;
    }
}
