using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour, IInteractable
{
    [SerializeField] string sceneToLoad;
    Canvas canvas;
    bool interacted;

    private void Start()
    {
        canvas = GetComponentInChildren<Canvas>(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        canvas.gameObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canvas.gameObject.SetActive(false);
    }

    public void Interact()
    {
        if (interacted)
            return;

        Manager.Scene.LoadScene(sceneToLoad);
        interacted = true;
    }
}
