using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using System;

public class EntityBehaviour : MonoBehaviour
{
    GameManager gameManager;
    public bool canInteract = true;
    public float maxSpeed = 1;
    public float speed = 1;
    public string serial = "";

    //event keyword before Action means only the script here can invoke, but others can still listen
    public Action<sbyte> TriggerAdmit;
    [SerializeField]
    sbyte admitted = 0;

    float waitTime = 1;
    public float timer = 10;

    Color[] colours;

    private void OnEnable()
    {
        TriggerAdmit += ChangeAdmitted;
    }
    private void OnDisable()
    {
        TriggerAdmit += ChangeAdmitted;
    }
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        colours = new Color[gameObject.transform.childCount + 1];
        colours[0] = GetComponent<SpriteRenderer>().color;
        for (int i = 1; i < transform.childCount; i++)
        {
            colours[i] = transform.GetChild(i).GetComponent<SpriteRenderer>().color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, -1 * speed * Time.deltaTime, 0);
        if (speed > 0)
        {
            timer -= waitTime * Time.deltaTime;
        }
        else if (speed <= 0 && timer <= 10)
        {
            timer += waitTime * Time.deltaTime;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        speed = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerBehaviour>() != null)
        {
            Actions.collision.Invoke(gameObject);
        }
    }

    public void ChangeAdmitted(sbyte admitValue)
    {
        admitted = admitValue;
        canInteract = false;
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        Destroy(rigid);
        Actions.SendSerial.Invoke(serial, admitted);
        StartCoroutine(Dissapear());
    }

    IEnumerator Dissapear()
    {
        speed = 1;
        gameManager.entityCount -= 1;

        float duration = 0.5f;
        float startTransparency = 1;
        float targetTransparency = 0;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = EasingFunctions.EaseInCubic(t);
            float a = Mathf.Lerp(startTransparency, targetTransparency, t);
            GetComponent<SpriteRenderer>().color = new Color(colours[0].r, colours[0].g, colours[0].b, a);
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(colours[i+1].r, colours[i+1].g, colours[0].b, a);
            }
            yield return null; // wait one frame
        }

        Destroy(gameObject);
    }
}
