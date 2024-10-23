using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSpawn : MonoBehaviour
{
    [SerializeField]
    private GameObject upgradeBoxPrefab;
    private Transform[] spawnPoints;
    private bool[] upgradeSpawned;
    [SerializeField]
    private float waitTime = 5.0f;
    private float currentTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = GetComponentsInChildren<Transform>();
        upgradeSpawned = new bool[spawnPoints.Length];
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            upgradeSpawned[i] = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime >= waitTime)
        {
            currentTime = 0.0f;
            int pos = GetFreePosition();
            if (pos >= 0)
            {
                Instantiate(upgradeBoxPrefab, spawnPoints[pos].position, Quaternion.identity);
                upgradeSpawned[pos] = true; // Remover essa linha para stackar múltiplas caixas no mesmo ponto. Falta checar se a caixa for destruída para poder spawnar outra
            }
        } else {
            currentTime += Time.deltaTime;
        }
    }

    int GetFreePosition()
    {
        int x = Random.Range(0, spawnPoints.Length);

        // Alterar para checar uma Sphere no ponto de spawn para detectar se já existe uma caixa spawnada nesse ponto
        if (upgradeSpawned[x])
        {
            x = -1;
            for(int i = 0; i < spawnPoints.Length; i++)
            {
                if (!upgradeSpawned[i])
                {
                    x = i;
                    break;
                }
            }
        }

        return x;
    }
}
