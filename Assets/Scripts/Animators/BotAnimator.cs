using Enumerations;
using UnityEngine;

namespace Animators
{
    public class BotAnimator : MonoBehaviour
    {
        public Animator animator;

        void Test()
        {
            // var moveDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if(Input.GetKeyDown(KeyCode.UpArrow))
                animator.SetTrigger("Up");
            if(Input.GetKeyDown(KeyCode.DownArrow))
                animator.SetTrigger("Down");
            if(Input.GetKeyDown(KeyCode.LeftArrow))
                animator.SetTrigger("Left");
            if(Input.GetKeyDown(KeyCode.RightArrow))
                animator.SetTrigger("Right");
        }

        public void ExecuteMove(Direction direction)
        {
            animator.SetTrigger(direction.ToString());
        }
    }
}
