using UnityEngine;

public partial class PlayerController
{
    private void UpdateAnimatorParameters()
    {
        float velocity = _isRunning && CurrentEnergy > 0 ? input.sqrMagnitude : input.sqrMagnitude * .5f;
        velocity *= (_controller.velocity.sqrMagnitude * (_isRunning ? .5f : 1)) / Speed;
        AnimatorCompo.SetFloat(velocityHash, velocity, .3f,
            Time.deltaTime);

        float maxValue = _isRunning && CurrentEnergy > 0 ? 1 : .5f;
        float horizontalInput = Mathf.Clamp(input.x, -maxValue, maxValue);
        float verticalInput = Mathf.Clamp(input.z, -maxValue, maxValue);
        AnimatorCompo.SetFloat(horizontalInputHash, horizontalInput, .1f,
            Time.deltaTime);
        AnimatorCompo.SetFloat(verticalInputHash, verticalInput, .1f,
            Time.deltaTime);
        AnimatorCompo.SetBool(isMovingHash, input.sqrMagnitude > 0);
    }

    public void OnAnimatorMove()
    {
        if (!UseRootMotion) return;
        Vector3 velocity = AnimatorCompo.deltaPosition;

        if (_isEvasion)
            velocity *= AnimatorCompo.GetFloat(rootMotionMultiplierHash) * rollSpeed;
        else
            velocity *= AnimatorCompo.GetFloat(rootMotionMultiplierHash);

        velocity.y = _velocityY;
        Move(velocity);
    }
}