using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyIndicator;

public class EnergyOrb : MonoBehaviour
{
    public int energyAmount = 10; 
    public int timeUntilDestroy = 30;
    public float magnetRange = 1f; // Range within which the orb will be attracted to troops
    public float baseSpeed = 10f;
    private Transform targetTroop;
    public float smoothTime = 0.1f;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        StartCoroutine(DestroySelf());
        //magnetRange = 1f;
    }

    private void Update()
    {
        FindClosestTroop();
        if (targetTroop != null)
        {
            //Debug.Log("");
            MoveTowardsTroop();
        }
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(timeUntilDestroy);

        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Troop"))
        {
            TroopEnergy otherTroopEnergy = other.GetComponent<TroopEnergy>();
            if (otherTroopEnergy != null && otherTroopEnergy.currentPower < otherTroopEnergy.maxPower)
            {
                FindObjectOfType<AudioManager>().Play("Orb");
                // Add energy to the troop
                if (otherTroopEnergy.currentPower + energyAmount > otherTroopEnergy.maxPower)
                {
                    otherTroopEnergy.currentPower = otherTroopEnergy.maxPower;
                }
                else
                {
                    otherTroopEnergy.currentPower += energyAmount;
                }
                otherTroopEnergy.UpdateHUD();

                otherTroopEnergy.Instruction5();

                // Optionally, play a sound, deactivate the energy orb, etc.
                Destroy(gameObject); // Destroy the energy orb after collision
            }

            /*
            // Check if the EnergySystem component exists on the other GameObject
            EnergySystem energySystem = FindObjectOfType<EnergySystem>(); // Find EnergySystem in the scene
            if (energySystem != null)
            {
                // Add energy to the EnergySystem
                energySystem.currentEnergy += energyAmount;
                energySystem.currentEnergy = Mathf.Clamp(energySystem.currentEnergy, 0f, energySystem.maxEnergy);

                // Optionally, update the UI directly if needed
                if (energySystem.energyText != null)
                {
                    energySystem.energyText.text = Mathf.FloorToInt(energySystem.currentEnergy).ToString();
                }

                energySystem.UpdateEnergyUI();
            }

            // Optionally, play a sound, deactivate the energy orb, etc.
            Destroy(gameObject); // Destroy the energy orb after collision with troop
            */
        }
    }

    private void FindClosestTroop()
    {
        Troop[] allTroops = FindObjectsOfType<Troop>();
        float closestDistance = magnetRange;
        Transform closestTroop = null;

        foreach (Troop troop in allTroops)
        {
            float distanceToTroop = Vector3.Distance(transform.position, troop.transform.position);
            if (distanceToTroop < closestDistance)
            {
                closestDistance = distanceToTroop;
                closestTroop = troop.transform;
            }
        }

        if (closestTroop != null && closestTroop.GetComponent<TroopEnergy>().currentPower < closestTroop.GetComponent<TroopEnergy>().maxPower)
        {
            targetTroop = closestTroop;
            //Debug.Log(targetTroop.name);
        }
    }

    private void MoveTowardsTroop()
    {
        if (targetTroop == null) return;

        float distanceToTroop = Vector3.Distance(transform.position, targetTroop.position);

        if (distanceToTroop > magnetRange || targetTroop.GetComponent<TroopEnergy>().currentPower >= targetTroop.GetComponent<TroopEnergy>().maxPower)
        {
            // If the troop is out of range, stop moving the orb
            velocity = Vector3.zero;
            return;
        }

        //Debug.Log($"Distance to troop: {distanceToTroop}, Magnet range: {magnetRange}");

        float speed = baseSpeed * (magnetRange - distanceToTroop) / magnetRange;

        transform.position = Vector3.SmoothDamp(transform.position, targetTroop.position, ref velocity, smoothTime, speed);
    }
}
