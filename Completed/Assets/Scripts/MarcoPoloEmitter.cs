using HoloToolkit.Unity;
using System.Collections;
using UnityEngine;

public class MarcoPoloEmitter : MonoBehaviour
{
    [SerializeField]
    private float distance = 2;
    [SerializeField]
    private float moveLength = 2;
    [SerializeField]
    private GameObject[] meshes;

    private Collider[] colliders;
    private Camera player;
    private Vector3 destination;

    private void Start()
    {
        this.player = Camera.main;
        this.colliders = this.gameObject.GetComponents<Collider>();
    }

    public void StartSounds()
    {
        UAudioManager.Instance.PlayEvent("MarcoPolo", this.gameObject);
    }

    private void OnSelected()
    {
        Transform playerTrans = this.player.transform;
        this.destination = playerTrans.position + (-playerTrans.forward * this.distance);
        this.destination += (playerTrans.right * Random.Range(-this.distance / 2, this.distance / 2));
        StartCoroutine(MoveToDestination(this.moveLength));
    }

    private IEnumerator MoveToDestination(float moveTime)
    {
        // Make the object invisible.
        for (int i = 0; i < this.meshes.Length; i++)
        {
            this.meshes[i].SetActive(false);
        }

        // Disable colliders so the cursor won't react to them.
        for (int i = 0; i < this.colliders.Length; i++)
        {
            this.colliders[i].enabled = false;
        }

        // Move the object to a new location.
        float timer = 0f;
        while (timer < moveTime)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            Vector3 deltaPos = (this.transform.position - destination) / (moveTime - timer) * Time.deltaTime;
            this.transform.position -= deltaPos;
        }

        this.transform.position = this.destination;
        this.transform.LookAt(this.player.transform);

        // Make the object visible.
        for (int i = 0; i < this.meshes.Length; i++)
        {
            this.meshes[i].SetActive(true);
        }

        // Enable colliders so the cursor will react to them.
        for (int i = 0; i < this.colliders.Length; i++)
        {
            this.colliders[i].enabled = true;
        }
    }
}