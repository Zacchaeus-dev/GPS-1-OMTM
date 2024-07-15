using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopPathfindArrow : MonoBehaviour
{
    public GameObject Troop;
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Troop") && other.gameObject == Troop )
        {
            gameObject.SetActive(false);
        }
    }

    public void bop()
    {
        StartCoroutine(Bopping());
    }

    public IEnumerator Bopping()
    {
        while (1==1)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + 0.02f);
            yield return new WaitForSeconds(0.1f);
            
            transform.position = new Vector2(transform.position.x, transform.position.y + 0.02f);
            yield return new WaitForSeconds(0.1f);

            transform.position = new Vector2(transform.position.x, transform.position.y - 0.02f);
            yield return new WaitForSeconds(0.1f);            
            
            transform.position = new Vector2(transform.position.x, transform.position.y - 0.02f);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
