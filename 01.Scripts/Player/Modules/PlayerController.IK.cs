using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public partial class PlayerController
{
    public void SetHandLIK(Transform target)
    {
        FullBodyBipedIKCompo.solver.leftHandEffector.target = target;
        float weight = Convert.ToByte(target);
        SetHandLIK(weight);
    }

    public void SetHandLIKImmediately(Transform target)
    {
        FullBodyBipedIKCompo.solver.leftHandEffector.target = target;
        float weight = Convert.ToByte(target);
        SetHandLIKImmediately(weight);
    }

    public void SetHandLIK(float weight, float duration = 0)
    {
        if (!FullBodyBipedIKCompo.solver.leftHandEffector.target) return;

        SetHandLIKImmediately(weight, duration);
    }

    public void SetHandLIKImmediately(float weight, float duration = 0)
    {
        if (weight == 1 && _isHitting) return;
        if (_handLIKSequence != null && _handLIKSequence.IsActive()) _handLIKSequence.Kill();
        _handLIKSequence = DOTween.Sequence();

        _handLIKSequence.Append(DOTween.To(() => FullBodyBipedIKCompo.solver.leftHandEffector.positionWeight,
            x => FullBodyBipedIKCompo.solver.leftHandEffector.positionWeight = x, weight, duration));
        _handLIKSequence.Join(DOTween.To(() => FullBodyBipedIKCompo.solver.leftHandEffector.rotationWeight,
            x => FullBodyBipedIKCompo.solver.leftHandEffector.rotationWeight = x, weight, duration));
    }

    public void SetHandRIK(Transform target)
    {
        FullBodyBipedIKCompo.solver.rightHandEffector.target = target;
        float weight = Convert.ToByte(target);
        SetHandRIK(weight);
    }

    public void SetHandRIKImmediately(Transform target)
    {
        FullBodyBipedIKCompo.solver.rightHandEffector.target = target;
        float weight = Convert.ToByte(target);
        SetHandRIKImmediately(weight);
    }

    public void SetHandRIK(float weight, float duration = 0)
    {
        if (!FullBodyBipedIKCompo.solver.rightHandEffector.target) return;
        SetHandRIKImmediately(weight, duration);
    }

    public void SetHandRIKImmediately(float weight, float duration = 0)
    {
        if (_handRIKSequence != null && _handRIKSequence.IsActive()) _handRIKSequence.Kill();
        _handRIKSequence = DOTween.Sequence();
        _handRIKSequence.Append(DOTween.To(() => FullBodyBipedIKCompo.solver.rightHandEffector.positionWeight,
            x => FullBodyBipedIKCompo.solver.rightHandEffector.positionWeight = x, weight, duration));
        _handRIKSequence.Join(DOTween.To(() => FullBodyBipedIKCompo.solver.rightHandEffector.rotationWeight,
            x => FullBodyBipedIKCompo.solver.rightHandEffector.rotationWeight = x, weight, duration));
    }

    private IEnumerator SetHandIKWeight1Coroutine()
    {
        yield return CoroutineHelper.WaitForSeconds(.1f);
        SetHandLIK(1, .5f);
        SetHandRIK(1, .5f);
    }

    public void SetHandIKWeight1()
    {
        if (!_isGuard)
        {
            if (LockOnUI.Instance && LockOnUI.Instance.gameObject.activeSelf) return;
            _setHandIKWeight1Coroutine = StartCoroutine(SetHandIKWeight1Coroutine());
        }
    }
}