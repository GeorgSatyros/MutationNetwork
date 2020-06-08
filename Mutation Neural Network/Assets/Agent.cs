using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public AgentManager manager;
    public GameObject limb;
    public float limbStrength = 5;
    public float limbCooldown = 1;
    float _currCooldown = 0;
    public GameObject bodyExtension;
    public MainBody mainBody;
    public GameObject target;
    public NeuralNetwork network;


    public bool hasTouchedTarget;

    // Start is called before the first frame update
    void Start()
    {
        if(manager==null)
            manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<AgentManager>();

        hasTouchedTarget = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!manager.hasSimulationStarted)
            return;

        if (_currCooldown <= 0)
        {
            TakeDecision();
            _currCooldown = limbCooldown;
        }
        _currCooldown -= Time.fixedDeltaTime;
    }

    void TakeDecision()
    {
        Vector3 targetDir = (target.transform.position - this.transform.position).normalized;
        Vector3 relativeLimbPosition = mainBody.transform.InverseTransformPoint(limb.transform.position);
        float distance = Vector3.Distance(target.transform.position, this.transform.position);
        float[] input = new float[] { targetDir.x, targetDir.y, targetDir.z, distance, limbStrength, relativeLimbPosition.x, relativeLimbPosition.y, relativeLimbPosition.z };
        float[] output = network.FeedForward(input);//Call to network to feedforward
        Vector3 _forceDir = new Vector3(output[0], output[1], output[2]).normalized;
        float forceStrength = Mathf.Clamp(output[3], 0f, limbStrength);
        bodyExtension.GetComponent<Rigidbody>().AddForceAtPosition(forceStrength * _forceDir, limb.transform.position, ForceMode.Impulse);
        Debug.DrawRay(limb.transform.position, forceStrength * _forceDir, Color.green);
    }

    public float UpdateFitness()
    {
        if (hasTouchedTarget)
            return 1;
        else
            return network.fitness = 1 / (target.transform.position - this.transform.position).sqrMagnitude;
    }
}
