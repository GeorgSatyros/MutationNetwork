using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public int agentSpread = 10;
    public float timeframe;
    public int populationSize;//creates population size
    public GameObject prefab;//holds bot prefab
    int _agentPosX = 0;
    int _agentPosZ = 0;

    public int[] layers = new int[3] { 5, 3, 2 };//initializing network to the right size

    [Range(0.0001f, 1f)] public float MutationChance = 0.01f;

    [Range(0f, 1f)] public float MutationStrength = 0.5f;

    [Range(0.1f, 10f)] public float Gamespeed = 1f;

    //public List<Bot> Bots;
    public List<NeuralNetwork> networks;
    private List<Agent> agents;
    internal bool hasSimulationStarted;

    void Start()// Start is called before the first frame update
    {
        //if (populationSize % 2 != 0)
        //    populationSize = 50;//if population size is not even, sets it to fifty
        hasSimulationStarted = false;
        StartCoroutine(StartSimulationInSeconds(5));
        InitNetworks();
        InvokeRepeating("CreateBots", 0.1f, timeframe);//repeating function
    }
    IEnumerator StartSimulationInSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        hasSimulationStarted = true;
    }

    public void InitNetworks()
    {
        networks = new List<NeuralNetwork>();
        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            //net.Load("Assets/Save.txt"); //on start load the network save
            networks.Add(net);
        }
    }

    public void CreateBots()
    {
        Time.timeScale = Gamespeed;//sets gamespeed, which will increase to speed up training

        if (agents != null)
        {
            for (int i = 0; i < agents.Count; i++)
            {
                GameObject.Destroy(agents[i].gameObject);//if there are Prefabs in the scene this will get rid of them
            }

            SortNetworks();//this sorts networks and mutates them
        }

        agents = new List<Agent>();
        for (int i = 0; i < populationSize; i++)
        {
            Vector3 position = new Vector3( _agentPosX , 0.5f, _agentPosZ);
            
            _agentPosX += agentSpread;
            if (i % 10 == 0)
            {
                _agentPosZ += agentSpread * 10;
                _agentPosX = 0;
            }
            Agent agent = Instantiate(prefab, position, Quaternion.identity).GetComponent<Agent>();//create botes
            agent.network = networks[i];//deploys network to each learner
            agents.Add(agent);
        }

        _agentPosZ = 0;
        _agentPosX = 0;

    }

    public void SortNetworks()
    {
        for (int i = 0; i < populationSize; i++)
        {
            agents[i].UpdateFitness(); //gets bots to set their corrosponding networks fitness
        }
       
        networks.Sort();
        networks.Reverse();
        networks[0].Save("Assets/Save.txt");//saves networks weights and biases to file, to preserve network performance
        
        for (int i = 0; i < populationSize / 2; i++)
        {
            networks[i] = networks[i + populationSize / 2].copy(new NeuralNetwork(layers));
            networks[i].Mutate((int)(1 / MutationChance), MutationStrength);
        }
    }
}