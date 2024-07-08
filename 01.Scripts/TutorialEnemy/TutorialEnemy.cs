using System;
using System.Collections;
using System.Collections.Generic;
using INab.Dissolve;
using UnityEngine;

public class TutorialEnemy : Enemy
{
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void ShowEnemy()
    {
        gameObject.SetActive(true);
        _dissolver.Materialize();
    }

    public void HideEnemy()
    {
        Die();
    }
}