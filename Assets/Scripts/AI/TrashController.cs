using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshObstacle))]
[RequireComponent(typeof(Collider))]
public class TrashController : MonoBehaviour
{
    [SerializeField]
    private GameObject pileOfTrashPrefab;
    [SerializeField]
    private GameObject trashBagPrefab;
    [SerializeField]
    private float solidifyTimer = 5;
    [SerializeField]
    private float switchToPileTimer = 25;
    [SerializeField]
    private float switchTransitionTime = 2f;
    public void Begin()
    {
        StartCoroutine(SwitchToPileOfTrash());
    }

    private IEnumerator SwitchToPileOfTrash()
    {
        //This is basically a timer
        yield return new WaitForSecondsRealtime(solidifyTimer);
        // Turn off rigidbody.
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        // Turn off collision.
        gameObject.GetComponent<Collider>().enabled = false;
        // Turn on nav mesh obstacle, makes NPC avoid this object instead of walking through it. 
        gameObject.GetComponent<NavMeshObstacle>().enabled = true;

        yield return new WaitForSecondsRealtime(switchToPileTimer);

        trashBagPrefab.SetActive(false);
        pileOfTrashPrefab.SetActive(true);
        // Turn off nav mesh obstacle to increase performance. 
        gameObject.GetComponent<NavMeshObstacle>().enabled = false;
    }
}
