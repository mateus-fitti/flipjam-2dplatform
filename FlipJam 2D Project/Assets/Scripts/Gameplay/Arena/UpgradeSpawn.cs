using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSpawn : MonoBehaviour
{
    [SerializeField]
    private GameObject upgradePrefab;
    private UpgradePickup upgrade;
    [SerializeField]
    private LayerMask upgradeLayer;
    [SerializeField]
    private UpgradeScriptableObject[] upgradeScriptableObjects;
    private Transform[] spawnPoints;
    [SerializeField]
    private float waitTime = 5.0f;
    private float currentTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = GetComponentsInChildren<Transform>();
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
                GameObject up = Instantiate(upgradePrefab, spawnPoints[pos].position, Quaternion.identity);
                upgrade = up.GetComponent<UpgradePickup>();
                int rng = Random.Range(0, upgradeScriptableObjects.Length);
                upgrade.SetUpgrade(upgradeScriptableObjects[rng]);
            }
        } else {
            currentTime += Time.deltaTime;
        }
    }

    int GetFreePosition()
    {
        int x = Random.Range(0, spawnPoints.Length);

        // Alterar para checar uma Sphere no ponto de spawn para detectar se já existe uma caixa spawnada nesse ponto
        if (Physics2D.OverlapCircle(spawnPoints[x].position, 5.0f, upgradeLayer))
        {
            x = -1;
            for(int i = 0; i < spawnPoints.Length; i++)
            {
                if (!Physics2D.OverlapCircle(spawnPoints[i].position, 5.0f, upgradeLayer))
                {
                    x = i;
                    break;
                }
            }
        }

        return x;
    }
}
