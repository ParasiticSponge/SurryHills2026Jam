using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.InputSystem.OnScreen.OnScreenStick;

public class GameManager : MonoBehaviour
{
    public GameObject tutorial;

    public Image screen;
    GameObject healthBar;
    GameObject camera;

    public Slider waveSlider;

    public GameObject entity;
    public List<Sprite> head;
    public List<Sprite> mask;
    public List<Sprite> top;
    public List<Sprite> bottom;
    public List<Sprite> hand;
    List<List<Sprite>> parts;

    GameObject player;
    public float health = 100;
    float damage = 20;

    float[] laneXPos = new float[] { -5.25f, -1.1f, 3.1f, 7.25f};
    float laneYPos = 6.6f;
    string[] partNames = new string[] { "head", "mask", "top", "bottom", "hand" };
    float[] partPositions = new float[] { 1.2f, 0.75f, -0.15f, -0.75f, 0 };

    public bool waveStart = false;
    int waveNum = 0;
    string waveSerial = "";
    int time = 120;
    int currentTime = 120;
    public float speed = 1f;

    public void OnEnable()
    {
        Actions.SendSerial += EvalSignal;
        Actions.Begin += StartGame;
    }
    public void OnDisable()
    {
        Actions.SendSerial -= EvalSignal;
        Actions.Begin -= StartGame;
    }
    private void Awake()
    {
        //Create a reference to the parts
        parts = new List<List<Sprite>> { head, mask, top, bottom, hand };
        player = FindObjectOfType<PlayerBehaviour>().gameObject;
        healthBar = FindObjectOfType<HealthBar>().gameObject;
        camera = FindObjectOfType<Camera>().gameObject;
        tutorial.SetActive(true);
    }

    // Start is called before the first frame update
    void StartGame()
    {
        waveSlider.value = 0;
        print("start");
        StartCoroutine(Begin());
    }

    IEnumerator Begin()
    {
        yield return StartCoroutine(EnterAnimation());
        yield return StartCoroutine(GameLogic());
    }
    IEnumerator EnterAnimation()
    {
        print("enter anim");
        screen.gameObject.SetActive(true);
        float duration = 1f;
        float start = 1f;
        float target = 0f;
        float elapsed = 0f;
        Color baseColour = screen.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = EasingFunctions.EaseOutCubic(t);
            float a = Mathf.Lerp(start, target, t);
            screen.color = new Color(baseColour.r, baseColour.g, baseColour.b, a);
            yield return null; // wait one frame
        }
        TutorialPage.changeNumber.Invoke(1);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if left mouse button was clicked down this frame
        if (Input.GetMouseButtonDown(0))
        {
            // Convert the mouse position from screen space to world space
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Cast a ray from the mouse position in any direction (Vector2.zero for a single point)
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);

            // Check if the ray hit any collider
            if (hit.collider != null && hit.collider.gameObject.GetComponent<LaneBehaviour>() != null)
            {
                //Debug.Log("Clicked on: " + hit.collider.gameObject.name);
                player.transform.position = hit.collider.transform.position;
            }
        }
    }
    void EvalSignal(string serial, sbyte status)
    {
        print(waveSerial);
        print(serial);
        switch (status)
        {
            case 1:
                if (serial == waveSerial)
                    StartCoroutine(TakeDamage());
                break;
            case -1:
                if (serial != waveSerial)
                    StartCoroutine(TakeDamage());
                break;
            default:
                break;
        }
    }

    IEnumerator TakeDamage()
    {
        camera.GetComponent<Shake>().start = true;
        float duration = 0.5f;
        float startHealth = health;
        float targetHealth = Mathf.Max(health - damage, 0f);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            //health = Mathf.Lerp(startHealth, targetHealth, elapsed / duration);
            float t = elapsed / duration;
            t = EasingFunctions.EaseOutCubic(t);
            health = Mathf.Lerp(startHealth, targetHealth, t);
            Slider slider = healthBar.GetComponent<Slider>();
            slider.value = Mathf.Lerp(startHealth, targetHealth, t);
            healthBar.GetComponent<HealthBar>().fill.color = healthBar.GetComponent<HealthBar>().gradient.Evaluate(slider.normalizedValue);

            yield return null; // wait one frame
        }

        health = targetHealth;

        if (health <= 0f)
            StartCoroutine(GameOver());
    }
    IEnumerator GameLogic()
    {
        print("game");
        if (health > 0)
        {
            yield return StartCoroutine(BeginWave());
            yield return StartCoroutine(SpawnLoop());
        }
    }

    IEnumerator Intro()
    {
        yield return new WaitForSeconds(3);
        waveStart = true;
    }

    IEnumerator BeginWave()
    {
        float duration = 1f;
        float start = 0;
        float target = waveSlider.maxValue;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = EasingFunctions.EaseOutCubic(t);
            waveSlider.value = Mathf.Lerp(start, target, t);
            yield return null; // wait one frame
        }

        //change wave number
        yield return new WaitForSeconds(2f);
        waveNum++;
        waveSerial = "";
        waveStart = true;
        for (int i = 0; i < partNames.Length; i++)
        {
            int randomiser = Random.Range(0, parts[i].Count);
            waveSerial += randomiser.ToString();
        }

        duration = 1f;
        start = waveSlider.maxValue;
        target = 0;
        elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = EasingFunctions.EaseOutCubic(t);
            waveSlider.value = Mathf.Lerp(start, target, t);
            yield return null; // wait one frame
        }
    }

    IEnumerator SpawnLoop()
    {
        float duration = 120f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return StartCoroutine(SpawnEntity());
        }

        StartCoroutine(GameLogic());
    }
    IEnumerator SpawnEntity()
    {
        CreateEntity();

        int waitTime = Random.Range(2, 6);
        // Pause for the allotted time
        yield return new WaitForSeconds(waitTime);
    }

    void CreateEntity()
    {
        //Choose a random lane
        int randomLane = Random.Range(0, 4);
        //Create the main entity body
        GameObject entityObject = Instantiate(entity, new Vector3(laneXPos[randomLane], 6.6f, -1), Quaternion.identity);
        EntityBehaviour behaviour = entityObject.GetComponent<EntityBehaviour>();
        behaviour.speed = speed;
        //Create the parts for the entity, choosing random clothes, using the reference list
        for (int i = 0; i < partNames.Length; i++)
        {
            GameObject part = new GameObject();
            part.name = partNames[i];

            part.transform.parent = entityObject.transform;
            part.transform.localPosition = new Vector3(0, partPositions[i], -1.1f);
            SpriteRenderer renderer = part.AddComponent<SpriteRenderer>();

            int randomiser = Random.Range(0, parts[i].Count);
            behaviour.serial += randomiser.ToString();
            renderer.sprite = parts[i][randomiser];
        }
        //print(behaviour.serial);
    }

    IEnumerator GameOver()
    {
        yield return 0;
    }
}
