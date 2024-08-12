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
        if (healthPercentage >= 100) return SSSRank;
        else if (healthPercentage >= 90) return SSRank;
        else if (healthPercentage >= 80) return SRank;
        else if (healthPercentage >= 20) return ARank;
        else if (healthPercentage >= 10) return BRank;
        else if (healthPercentage >= 5) return CRank;
        else if (healthPercentage >= 2.5f) return DRank;
        else if (healthPercentage > 0) return ERank;
        else return FRank;
    }
}

