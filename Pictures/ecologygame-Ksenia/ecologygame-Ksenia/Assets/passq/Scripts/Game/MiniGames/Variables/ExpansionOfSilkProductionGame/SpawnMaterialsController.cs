using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMaterialsController : MonoBehaviour
{
    public static SpawnMaterialsController Instance { get; private set; }

    [SerializeField] private GameObject workShopPrefab;
    [SerializeField] private GameObject materialsPrefab;
    [SerializeField] private MeshCollider spawnPanel;
    [SerializeField] private Sprite[] arrSpriteWorkShop;
    [SerializeField] private float centerOffset;
    [SerializeField] private float materialOffset;
    [SerializeField] private int countMaterial = 10;

    private List<Vector2> spawnList = new();

    [SerializeField] private float tapRadius = 0.05f;
    private bool isEnabled = true;
    private int counter = 0;

    GameObject workShop;

    private void Awake()
    {
        Instance = this;
    }

    public void SetEnable(bool value)
    {
        isEnabled = value;
    }

    private void Update()
    {
        if (!isEnabled)
            return;

        if (Input.GetMouseButtonDown(0))
            ExecuteMapTap(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private void ExecuteMapTap(Vector2 pos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, tapRadius);

        foreach (var item in hits)
            if (item.TryGetComponent<TapMaterialController>(out TapMaterialController tapable))
                tapable.OnTap();
    }
    // Start is called before the first frame update
    void Start()
    {
        initWorkShop();
        for (int i = 0; i < countMaterial; i++)
            initMaterials();
    }

    float x, y;
    Vector2 spawnPos;

    private void initWorkShop()
    {
        workShop = Instantiate(workShopPrefab, spawnPanel.transform.position, Quaternion.identity);
        workShop.transform.parent = spawnPanel.transform;
        workShop.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        workShop.GetComponent<TapMaterialController>().SetTapsToComplete(arrSpriteWorkShop.Length);
        workShop.GetComponent<TapMaterialController>().OnTapped.AddListener(delegate { upWorkShop(); });

    }

    public void initMaterials(int count = 0)
    {
        x = Random.Range(spawnPanel.transform.position.x - Random.Range(0, spawnPanel.bounds.extents.x), spawnPanel.transform.position.x + Random.Range(0, spawnPanel.bounds.extents.x));
        y = Random.Range(spawnPanel.transform.position.z - Random.Range(0, spawnPanel.bounds.extents.z), spawnPanel.transform.position.z + Random.Range(0, spawnPanel.bounds.extents.z));
        spawnPos = new Vector3(x, y);

        bool check = CheckSpawnPoint(spawnPos);

        if (check)
        {
            GameObject player = Instantiate(materialsPrefab, spawnPos, Quaternion.identity);
            player.transform.parent = spawnPanel.transform;
            player.GetComponent<TapMaterialController>().OnCompleteTapping.AddListener(delegate { Move(player.transform); });
            //player.name = "Игрок";
            //InitBotHouseBuilding(SpawnPos);
            //Debug.LogError("Позиция сгенерирована! " + spawnPos);
        }
        else if (count < 25)
        {
            count++;
            initMaterials(count);
            //Debug.Log("Смена позиции");
        }
        //else
        //{
        //    //generateBots.interactable = true;
        //    Debug.LogError("Не удалось сгенерировать сдание");
        //}
    }

    bool CheckSpawnPoint(Vector2 spawnPos)
    {
        if (Vector2.Distance(spawnPos, new Vector2(spawnPanel.transform.position.x, spawnPanel.transform.position.y)) < centerOffset)
        {
            return false;
        }

        foreach (Vector2 point in spawnList)
        {
            if (Vector2.Distance(spawnPos, point) < materialOffset)
            {
                return false;
            }
        }

        spawnList.Add(spawnPos);
        return true;
    }

    private void upWorkShop()
    {
        int num = workShop.GetComponent<TapMaterialController>().GetTapsToComplete();
        if (arrSpriteWorkShop.Length > num)
            workShop.GetComponent<SpriteRenderer>().sprite = arrSpriteWorkShop[num];
        else
            Destroy(workShop);
    }

    public void Move(Transform material)
    {
        counter++;
        StartCoroutine(directPathMoving(material));
        //Debug.LogError(counter + " из " + countMaterial);
        if (counter == countMaterial)
            workShop.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    IEnumerator directPathMoving(Transform material)
    {
        int stepMove = 10;

        float cof_x = spawnPanel.transform.position.x - material.position.x;
        float cof_y = spawnPanel.transform.position.y - material.position.y;

        cof_x = cof_x / stepMove;
        cof_y = cof_y / stepMove;

        for (int i = 0; i < stepMove; i++)
        {
            yield return new WaitForSeconds(0.01f);
            material.Translate(cof_x, cof_y, 0);
        }
        yield return new WaitForSeconds(0.05f);
    }
}
