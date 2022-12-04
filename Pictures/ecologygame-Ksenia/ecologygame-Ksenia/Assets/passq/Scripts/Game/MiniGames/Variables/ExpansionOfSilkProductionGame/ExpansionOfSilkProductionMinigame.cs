using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpansionOfSilkProductionMinigame : MinigameBase
{
    [SerializeField] private GameObject materialsPrefab;
    [SerializeField] private GameObject workShopPrefab;
    [SerializeField] private bool Accept;
    [SerializeField] private Sprite[] arrSpriteWorkShop;
    [SerializeField] private MeshCollider spawnPanel;
    [SerializeField] private GameObject dustAnimations;

    [SerializeField] private float centerOffset;
    [SerializeField] private float materialOffset;
    [SerializeField] private int countMaterial = 10;
    [SerializeField] private float tapRadius = 0.05f;

    //private PlacableObject currentField = null;
    private GameObject workShop = null;

    private bool isEnabled = false;
    private int counter = 0;
    private List<Vector2> spawnList = new();

    private float x, y;
    private Vector2 spawnPos;

    protected override void OnGameStarted()
    {
        base.OnGameStarted();

        initWorkShop();
        if (Accept)
        {
            for (int i = 0; i < countMaterial; i++)
                initMaterials();
        }

        isEnabled = true;
    }

    private void GameEnded()
    {
        isEnabled = false;
        EndGame();
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

    //private void Update()
    //{
    //    if (!isEnabled)
    //        return;

    //    if (currentField == null)
    //        GetPlacable();

    //    //MovePlacable(Camera.main.ScreenToWorldPoint(Input.mousePosition));

    //    if (Input.GetMouseButtonDown(0))
    //        TryPlace();
    //}

    //private void GetPlacable()
    //{
    //    if (fieldsCount <= 0)
    //    {
    //        EndGame();
    //        return;
    //    }

    //    currentField = Instantiate(materialsPrefab, fieldContainer);

    //    fieldsCount--;
    //}

    //private void MovePlacable(Vector2 pos)
    //{
    //    if (currentField == null)
    //        return;

    //    currentField.transform.position = pos;
    //}

    //private void TryPlace()
    //{
    //    if (currentField == null)
    //        return;

    //    if (!currentField.TryPlace())
    //        return;

    //    currentField = null;
    //}

    private void initWorkShop()
    {
        workShop = Instantiate(workShopPrefab, spawnPanel.transform.position, Quaternion.identity);
        workShop.transform.parent = spawnPanel.transform;
        workShop.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        workShop.GetComponent<TapMaterialController>().OnTapped.AddListener(delegate { upWorkShop(); });

        if (!Accept)
        {
            workShop.GetComponent<TapMaterialController>().SetTapsToComplete(arrSpriteWorkShop.Length);
        }
        else
        {
            workShop.GetComponent<TapMaterialController>().SetTapsToComplete(0);
        }

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
        else
        {
            initMaterials();
        }
        //else if (count < 25)
        //{
        //    count++;
        //    initMaterials(count);
        //    //Debug.Log("Смена позиции");
        //}
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
        {
            Destroy(workShop);
            GameEnded();
        }
    }

    public void Move(Transform material)
    {
        counter++;
        StartCoroutine(directPathMoving(material));
        //Debug.LogError(counter + " из " + countMaterial);
        if (counter == countMaterial)
        {
            StartCoroutine(constructionAnimation());
            //foreach (Transform child in spawnPanel.transform)
            //{
            //    GameObject.Destroy(child.gameObject);
            //}
        }
    }

    IEnumerator directPathMoving(Transform material)
    {
        int stepMove = 10;

        float cof_x = spawnPanel.transform.position.x - material.position.x;
        float cof_y = spawnPanel.transform.position.y - material.position.y;

        cof_x = cof_x / stepMove;
        cof_y = cof_y / stepMove;

        Vector3 size = workShop.transform.localScale;
        workShop.transform.localScale = new Vector3(size.x * 1.2f, size.y * 1.2f, size.z * 1.2f);

        for (int i = 0; i < stepMove; i++)
        {
            yield return new WaitForSeconds(0.01f);
            material.Translate(cof_x, cof_y, 0);
        }

        workShop.transform.localScale = new Vector3(size.x, size.y, size.z);
        yield return new WaitForSeconds(0.05f);
    }

    IEnumerator constructionAnimation()
    {
        workShop.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        dustAnimations.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        dustAnimations.SetActive(false);
        workShop.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        yield return new WaitForSeconds(0.25f);
        GameEnded();
    }
}
