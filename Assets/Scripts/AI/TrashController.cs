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

        // Set the same parent as this object to trashBadPrefab
        trashBagPrefab.transform.parent = transform.parent;

        // Start the pile of trash prefab but move its y position down a little bit
        pileOfTrashPrefab.SetActive(true);
        // Reset rotation and position in y axis for trash bag. 
        transform.localEulerAngles = Vector3.zero;
        transform.localPosition = new Vector3(transform.localPosition.x, -0.5f, transform.localPosition.z);
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        // Set time value and start and end positions for trash bag and pile of trash respectively. 
        float t = 0f;
        Vector3 startPos_bag = trashBagPrefab.transform.localPosition;    
        Vector3 endPos_bag = new Vector3(trashBagPrefab.transform.localPosition.x, -1, trashBagPrefab.transform.localPosition.z);
        Vector3 startPos_pile = transform.localPosition;
        Vector3 endPos_pile = transform.localPosition = new Vector3(transform.localPosition.x, 0.5f, transform.localPosition.z);
        // Creates the transition between the trash bad and the pile of trash
        while (t < switchTransitionTime)
        {
            trashBagPrefab.transform.localPosition = Vector3.Lerp(startPos_bag, endPos_bag, t / switchTransitionTime);
            transform.localPosition = Vector3.Lerp(startPos_pile, endPos_pile, t / switchTransitionTime);
            t += Time.deltaTime;
            yield return null;
        }

        // Turn off nav mesh obstacle to increase performance. 
        gameObject.GetComponent<NavMeshObstacle>().enabled = false;
        // Disable the trashbag
        trashBagPrefab.SetActive(false);
    }
}
