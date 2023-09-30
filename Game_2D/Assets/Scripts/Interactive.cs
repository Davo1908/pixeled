using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactive : MonoBehaviour
{
    private bool canInteract;
    private BoxCollider2D bc;
    private SpriteRenderer sp;
    private GameObject interactableIndicator;
    private Animator anim;

    public UnityEvent uEvent;

    public GameObject[] objects;

    public bool isChest;
    public bool isLever;
    public bool leverOperated;
    public bool isCheckpoint;
    public bool isSelector;

    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        if (transform.GetChild(0) != null)
            interactableIndicator = transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract= true;
            interactableIndicator.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract= false;
            interactableIndicator.SetActive(false);
        }
    }

    private void Chest()
    {
        if (isChest)
        {
            Instantiate(objects[Random.Range(0, objects.Length)], transform.position, Quaternion.identity);
            anim.SetBool("open", true);
            bc.enabled = false;
        }
    }

    private void Lever()
    {
        if (isLever && !leverOperated)
        {
            anim.SetBool("activated", true);
            leverOperated= true;
            uEvent.Invoke();
            interactableIndicator.SetActive(false);
            bc.enabled= false;
            this.enabled= false;
        }
    }

    private void Checkpoint()
    {
        if(isCheckpoint)
        {
            uEvent.Invoke();
            GameManager.Instance.SaveGame();
        }
    }

    private void LevelSelector()
    {
        if (isSelector)
        {
            uEvent.Invoke();
        }
    }

    private void Update()
    {
        if(canInteract && Input.GetKeyDown(KeyCode.C))
        {
            Chest();
            Lever();
            Checkpoint();
            LevelSelector();
        }
    }
}
