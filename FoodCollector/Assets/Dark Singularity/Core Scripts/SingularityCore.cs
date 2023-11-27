using UnityEngine;
[RequireComponent(typeof(SphereCollider))]
public class SingularityCore : MonoBehaviour
{
    // The rate at which the objects scale down
    private float scaleDownRate = 0.1f;

    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<SingularityPullable>())
        {
            // Gradually scale down the object
            Vector3 newScale = other.transform.localScale - new Vector3(scaleDownRate, scaleDownRate, scaleDownRate);
            other.transform.localScale = newScale;

            // If the object is small enough, deactivate it
            if (newScale.x <= 0.1f)
            {
                other.gameObject.SetActive(false);
            }
        }
    }
    void Start(){
        if(GetComponent<SphereCollider>()){
            GetComponent<SphereCollider>().isTrigger = true;
        }
    }
}
