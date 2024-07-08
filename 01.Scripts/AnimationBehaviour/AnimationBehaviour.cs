    using UnityEngine;

    public class AnimationBehaviour : StateMachineBehaviour
    {
        protected PlayerController _player;
        
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _player = animator.transform.parent.GetComponent<PlayerController>();
            _player.stopRotate = true;
        }

    }
