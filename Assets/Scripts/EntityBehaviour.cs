using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using System;
using UnityEngine.UI;

public class EntityBehaviour : MonoBehaviour
{
    GameManager gameManager;
    public bool canInteract = true;
    public float maxSpeed = 1;
    public float speed = 1;
    public string serial = "";

    GameObject hourglass;
    GameObject hourMask;

    //event keyword before Action means only the script here can invoke, but others can still listen
    public Action<sbyte> TriggerAdmit;
    [SerializeField]
    sbyte admitted = 0;

    float waitTime = 1;
    float maxTime = 3;
    public float timer = 3;
    bool showTimer = false;
    int collisionCount = 0;

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
        hourglass = transform.Find("Timer").gameObject;
        hourMask = hourglass.transform.Find("Sprite Mask").gameObject;
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
        if (timer < maxTime && !showTimer)
        {
            showTimer = true;
            hourglass.SetActive(true);
        }
        else if (timer >= maxTime && showTimer)
        {
            showTimer = false;
            hourglass.SetActive(false);
        }
        transform.position += new Vector3(0, -1 * speed * Time.deltaTime, 0);
        //if not moving, become impatient
        if (speed <= 0)
        {
            timer -= waitTime * Time.deltaTime;
            //0.8 is the initial position
            float movementPerFrame = 0.25f * Time.deltaTime;
            //decrease hour glass fill mask
            hourMask.transform.position -= new Vector3(0, movementPerFrame, 0);
        }

        else if (speed > 0 && timer < maxTime)
        {
            timer += waitTime * Time.deltaTime;
            float movementPerFrame = 0.25f * Time.deltaTime;
            hourMask.transform.position += new Vector3(0, movementPerFrame, 0);
        }
        if (timer > maxTime)
        {
            timer = maxTime;
            float pox = hourMask.transform.localPosition.x;
            float poz = hourMask.transform.localPosition.z;
            hourMask.transform.localPosition = new Vector3(pox, 0.8f, poz);
        }

        if (timer <= 0)
        {
            Actions.Angry.Invoke();
            StartCoroutine(Dissapear());
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        print("enter place");
        if (collision.gameObject.GetComponent<EntityBehaviour>() != null)
        {
            print("collision with entity");
            collisionCount++;
        }
        speed = 0;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        print("exit");
        if (collision.gameObject.GetComponent<EntityBehaviour>() != null)
        {
            print("exit entity");
            collisionCount--;
        }

        if (collisionCount <= 1)
        {
            print("collision <= 1");
            //collisionCount = 0;
            speed = gameManager.speed;
        }
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
                if (transform.GetChild(i).name != "Timer") transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(colours[i+1].r, colours[i+1].g, colours[0].b, a);
            }
            yield return null; // wait one frame
        }

        Destroy(gameObject);
    }
}
