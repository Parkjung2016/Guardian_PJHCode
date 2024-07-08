using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractAble
{
    public void Interact();
    public string InteractName { get; set; }
    public bool CanInteract { get; set; }
    public Transform Trm { get; set; }
    
}