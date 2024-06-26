using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractAble : MonoBehaviour
{
    public abstract void Interact();
    public string interactName;
    public bool canInteract = true;
}