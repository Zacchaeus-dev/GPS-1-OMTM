using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class RankingSystem : MonoBehaviour
{
    public GameObject killdozer;

    public GameObject SSSRank;
    public GameObject SSRank;
    public GameObject SRank;
    public GameObject ARank;
    public GameObject BRank;
    public GameObject CRank;
    public GameObject DRank;
    public GameObject ERank;
    public GameObject FRank;

    private int maxHealth = 500;
    private int currentHealth;

    void Start()
    {
        UpdateRank();
    }

    void Update()
    {
        currentHealth = killdozer.GetComponent<Killdozer>().currentHealth;
        UpdateRank();
    }

    void UpdateRank()
    {
        float healthPercentage = (float)currentHealth / maxHealth * 100;
        SetActiveRank(CalculateRank(healthPercentage));
    }

    void SetActiveRank(GameObject rank)
    {
        // Deactivate all rank GameObjects
        SSSRank.SetActive(false);
        SSRank.SetActive(false);
        SRank.SetActive(false);
        ARank.SetActive(false);
        BRank.SetActive(false);
        CRank.SetActive(false);
        DRank.SetActive(false);
        ERank.SetActive(false);
        FRank.SetActive(false);

        // Activate the correct rank GameObject
        if (rank != null)
        {
            rank.SetActive(true);
        }
    }

    GameObject CalculateRank(float healthPercentage)
    {
        if (healthPercentage == 100) return SSSRank;
        else if (healthPercentage > 95) return SSRank;
        else if (healthPercentage > 90) return SRank;
        else if (healthPercentage > 80) return ARank;
        else if (healthPercentage > 70) return BRank;
        else if (healthPercentage > 60) return CRank;
        else if (healthPercentage > 50) return DRank;
        else if (healthPercentage > 35) return ERank;
        else return FRank;
    }
}

