using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    Volume volume;
    ColorAdjustments colorAdjustments;

    GameObject canvas;
    [SerializeField]
    Animator animator;
    public Text waveTimer;
    public GameObject tutorial;
    public GameObject cardDeck;
    public GameObject card1;
    public GameObject card2;
    public GameObject waveBoard;
    public GameObject waveOldNum;
    public GameObject waveNewNum;
    public Sprite deckFront;
    public Sprite deckBack;
    public GameObject outfitReveal;

    public Image screen;
    GameObject healthBar;
    GameObject camera;

    float entitySpawnZ = -9;
    float spawnZ = -9;
    float zNum = 0.001f;
    public int entityCount = 0;
    int spawnRateMin = 20;
    int spawnRateMax = 30;
    public float speed = 1f;

    public Slider waveSlider;

    public GameObject entity;
    public List<Sprite> top;
    public List<Sprite> hand;
    public List<Sprite> accessory;
    public List<Sprite> hair;
    public List<Sprite> mask;
    public List<Sprite> head;
    List<List<Sprite>> parts;

    GameObject player;
    public float health = 100;
    float damage = 20;

    float[] laneXPos = new float[] { -5.25f, -1.1f, 3.1f, 7.25f};
    float laneYPos = 6.6f;
    string[] partNames = new string[] { "top", "hand", "accessory", "hair", "mask", "head" };
    float[] partPositions = new float[] { -0.197f, 0, 0.111f, 0.741f, 0.558f, 1.02f };
    float[] partZIndex = new float[] { -0.01f, -0.02f, -0.03f, -0.04f, -0.05f, -0.06f };
    Vector3[] partScales = new Vector3[] { new Vector3(1, 1, 1), new Vector3(1, 1, 1), new Vector3(1.1f, 0.7f, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 0.8f, 1) };

    public bool waveStart = false;
    int waveNum = 0;
    string waveSerial = "";
    float time = 30;
    float currentTime = 30;
    bool cardRevealed = false;

    bool canPause = true;
    bool gamePaused = false;
    bool endGame = false;

    public void OnEnable()
    {
        Actions.SendSerial += EvalSignal;
        Actions.Begin += StartGame;
        Actions.pauseGame += Pause;
        Actions.restartGame += ResetGame;
        Actions.Angry += EntityAngry;
    }
    public void OnDisable()
    {
        Actions.SendSerial -= EvalSignal;
        Actions.Begin -= StartGame;
        Actions.pauseGame -= Pause;
        Actions.restartGame -= ResetGame;
        Actions.Angry -= EntityAngry;
    }
    private void Awake()
    {
        //Create a reference to the parts
        parts = new List<List<Sprite>> { top, hand, accessory, hair, mask, head };
        player = FindObjectOfType<PlayerBehaviour>().gameObject;
        healthBar = FindObjectOfType<HealthBar>().gameObject;
        camera = FindObjectOfType<Camera>().gameObject;
        canvas = FindObjectOfType<Canvas>().gameObject;
        volume = FindObjectOfType<Volume>();
        animator = canvas.GetComponent<Animator>();
        tutorial.SetActive(true);

        player.transform.position = new Vector3(-3.15f, -2.72f, player.transform.position.z);
    }
    public void Pause()
    {
        if (canPause)
        {
            if (!gamePaused)
            {
                gamePaused = true;
                Time.timeScale = 0f;
                if (volume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
                {
                    colorAdjustments.saturation.value = -100f;
                }
            }
            else
            {
                gamePaused = false;
                Time.timeScale = 1f;
                if (volume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
                {
                    colorAdjustments.saturation.value = 0f;
                }
            }
        }
    }

    private void ResetGame()
    {
        //Time.timeScale = 1;
        //speed = 1f;
        //spawnRateMin = 20;
        //spawnRateMax = 30;
        //waveNum = 0;
        //StartGame();
        SceneManager.LoadScene(1);
    }

    // Start is called before the first frame update
    void StartGame()
    {
        waveSlider.value = 0;
        StartCoroutine(Begin());
    }

    IEnumerator Begin()
    {
        yield return StartCoroutine(EnterAnimation());
        yield return StartCoroutine(GameLogic());
    }
    IEnumerator EnterAnimation()
    {
        canPause = true;
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

            int laneMask = LayerMask.GetMask("Lane");

            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, 0f, laneMask);

            if (hit.collider != null)
            {
                player.transform.position = hit.collider.transform.position;
            }

            //// Cast a ray from the mouse position in any direction (Vector2.zero for a single point)
            //RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);

            //// Check if the ray hit any collider
            //if (hit.collider != null && hit.collider.gameObject.GetComponent<LaneBehaviour>() != null)
            //{
            //    //Debug.Log("Clicked on: " + hit.collider.gameObject.name);
            //    player.transform.position = hit.collider.transform.position;
            //}
        }
        if (card1.GetComponent<RectTransform>().eulerAngles.y >= 90 && cardRevealed == false)
        {
            cardRevealed = true;
            card1.GetComponent<Image>().sprite = deckFront;
            GameObject outfit1 = Instantiate(outfitReveal, card1.transform);
            //top
            outfit1.transform.GetChild(1).GetComponent<Image>().sprite = parts[0][waveSerial[0] - '0'];
            //hand
            outfit1.transform.GetChild(2).GetComponent<Image>().sprite = parts[1][waveSerial[1] - '0'];
            //acc
            outfit1.transform.GetChild(3).GetComponent<Image>().sprite = parts[2][waveSerial[2] - '0'];
            //hair
            outfit1.transform.GetChild(4).GetComponent<Image>().sprite = parts[3][waveSerial[3] - '0'];
            //mask
            outfit1.transform.GetChild(5).GetComponent<Image>().sprite = parts[4][waveSerial[4] - '0'];
            //head
            outfit1.transform.GetChild(6).GetComponent<Image>().sprite = parts[5][waveSerial[5] - '0'];
        }
        if (waveStart && currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            int minutes = (int)(currentTime / 60);
            int seconds = (int)(currentTime % 60);
            string minShow = minutes.ToString();
            string secShow = seconds.ToString();
            if (minutes < 10) minShow = "0" + minShow;
            if (seconds < 10) secShow = "0" + secShow;
            waveTimer.text = minShow + ":" + secShow;
        }
    }
    void EvalSignal(string serial, sbyte status)
    {
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
    void EntityAngry()
    {
        StartCoroutine(TakeDamage());
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
        yield return StartCoroutine(BeginWave());
        yield return StartCoroutine(SpawnLoop());
    }

    IEnumerator Intro()
    {
        yield return new WaitForSeconds(3);
        waveStart = true;
    }

    IEnumerator BeginWave()
    {
        spawnZ = entitySpawnZ;
        waveSerial = "";
        for (int i = 0; i < partNames.Length; i++)
        {
            int randomiser = Random.Range(0, parts[i].Count);
            waveSerial += randomiser.ToString();
        }

        currentTime = time;
        waveTimer.text = "00:00";
        //increase entity speed
        speed += 0.5f;
        //decrease spawn rate
        spawnRateMin -= 1;
        spawnRateMax -= 1;

        Color current = cardDeck.GetComponent<Image>().color;
        cardDeck.GetComponent<Image>().color = new Color(current.r, current.g, current.b, 0);
        current = card1.GetComponent<Image>().color;
        card1.GetComponent<Image>().color = new Color(current.r, current.g, current.b, 0);
        Transform child = card1.transform.Find("DeckImage(Clone)");
        if (child != null)
            Destroy(child.gameObject);
        card1.GetComponent<Image>().sprite = deckBack;
        current = card2.GetComponent<Image>().color;
        card2.GetComponent<Image>().color = new Color(current.r, current.g, current.b, 0);
        current = waveBoard.GetComponent<Image>().color;
        waveBoard.GetComponent<Image>().color = new Color(current.r, current.g, current.b, 0);
        current = waveOldNum.GetComponent<Text>().color;
        waveOldNum.GetComponent<Text>().color = new Color(current.r, current.g, current.b, 0);
        current = waveNewNum.GetComponent<Text>().color;
        waveNewNum.GetComponent<Text>().color = new Color(current.r, current.g, current.b, 0);

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

        waveOldNum.GetComponent<Text>().text = waveNum.ToString();
        waveNum++;
        waveNewNum.GetComponent<Text>().text = waveNum.ToString();

        animator.Play("CardReveal", 0, 0f);
        cardRevealed = false;

        //float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(2.8f);

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
        waveStart = true;
    }

    IEnumerator SpawnLoop()
    {
        while (currentTime > 0)
        {
            yield return StartCoroutine(SpawnEntity());
        }

        //wait until all entities destroyed
        while (entityCount > 0)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        waveStart = false;
        StartCoroutine(GameLogic());
    }
    IEnumerator SpawnEntity()
    {
        CreateEntity();
        entityCount++;
        spawnZ += zNum;

        float waitTime = Random.Range(spawnRateMin, spawnRateMax) / 10;
        // Pause for the allotted time
        yield return new WaitForSeconds(waitTime);
    }

    void CreateEntity()
    {
        //Choose a random lane
        int randomLane = Random.Range(0, 4);
        //Create the main entity body
        GameObject entityObject = Instantiate(entity, new Vector3(laneXPos[randomLane], 6.6f, spawnZ), Quaternion.identity);
        EntityBehaviour behaviour = entityObject.GetComponent<EntityBehaviour>();
        behaviour.speed = speed;

        //chance to spawn unwanted entity
        //25%
        int randomBad = Random.Range(0, 4);

        if (randomBad == 3)
        {
            //Choose the parts that equal the unwanted outfit
            for (int i = 0; i < partNames.Length; i++)
            {
                GameObject part = new GameObject();
                part.name = partNames[i];

                part.transform.parent = entityObject.transform;
                part.transform.localPosition = new Vector3(0, partPositions[i], partZIndex[i]);
                part.transform.localScale = partScales[i];
                SpriteRenderer renderer = part.AddComponent<SpriteRenderer>();

                //convert char to int
                int outfitNum = waveSerial[i] - '0';
                behaviour.serial += outfitNum.ToString();
                renderer.sprite = parts[i][outfitNum];
            }
        }
        else
        {
            //Create the parts for the entity, choosing random clothes, using the reference list
            for (int i = 0; i < partNames.Length; i++)
            {
                GameObject part = new GameObject();
                part.name = partNames[i];

                part.transform.parent = entityObject.transform;
                part.transform.localPosition = new Vector3(0, partPositions[i], partZIndex[i]);
                part.transform.localScale = partScales[i];
                SpriteRenderer renderer = part.AddComponent<SpriteRenderer>();

                int randomiser = Random.Range(0, parts[i].Count);
                behaviour.serial += randomiser.ToString();
                renderer.sprite = parts[i][randomiser];
            }
        }
        //print(behaviour.serial);
    }

    IEnumerator GameOver()
    {
        canPause = false;
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(1f);

        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.Play("GameOver", 0, 0f);
        //yield return new WaitForSeconds(2.5f);

        //animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }
}
