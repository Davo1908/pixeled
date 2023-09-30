using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FinalDetection : MonoBehaviour
{
    public bool advanced;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.ActiveTransitionPanel();
            GameManager.Instance.advancingLevel = advanced;
            StartCoroutine(WaitForPositionChange());
        }
    }

    private IEnumerator WaitForPositionChange()
    {
        yield return new WaitForSeconds(0.1f);
        GameManager.Instance.ChangePlayerPosition();
        if(advanced)
            GameManager.Instance.currentLevel++;
        else
            GameManager.Instance.currentLevel--;
    }
}
