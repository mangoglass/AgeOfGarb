using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoHomeBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        NPCController controller = animator.gameObject.GetComponent<NPCController>();
        controller.agent.isStopped = true;
        // Remove the animated trash bad. 
        if (stateInfo.IsName("ThrowTrash"))
        {
            // The delay is added to let the trash animate before it's removed.
            Destroy(controller.animatedTrash, 1.5f);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // After falling down and throwing the trash in the air, call GoHome to start running back home again. 
        NPCController controller = animator.gameObject.GetComponent<NPCController>();
        if (!animator.GetBool("GameOver"))
        {
            controller.GoHome();
            controller.agent.isStopped = false;
        }
    }
}