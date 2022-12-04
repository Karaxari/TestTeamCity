using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErasingController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer puzzlePicture;
    [SerializeField] Sprite picture;
    [SerializeField] private ErasingPicture erasingPicturePrefab;
    [SerializeField] int eraserPercentage;
    [SerializeField] Sprite pictureToErase;

    public ErasingPicture crntErasingPicture;

    void Start()
    {
        puzzlePicture.sprite = picture;

        crntErasingPicture = Instantiate(erasingPicturePrefab, puzzlePicture.gameObject.transform);
        crntErasingPicture.Setup(eraserPercentage, pictureToErase);
      //  crntErasingPicture.gameObject.transform.localScale += puzzlePicture.gameObject.transform.localScale;
        crntErasingPicture.gameObject.transform.localPosition = new Vector3(0, 0 ,- 1);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
