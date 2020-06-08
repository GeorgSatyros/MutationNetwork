using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBody : MonoBehaviour
{
    Agent parentAgent;
    // Start is called before the first frame update
    void Start()
    {
        parentAgent = GetComponentInParent<Agent>();
        parentAgent.mainBody = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == parentAgent.target)
        {
            parentAgent.hasTouchedTarget = true;
            Debug.Log("Agent "+parentAgent.name+" touched the target!");
        }
    }

}
