using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommodityPriceRegulation : MonoBehaviour
{
    [SerializeField] private int price;
    [SerializeField] private TMP_Text priceProduct;
    [SerializeField] private Slider priceCof;

    // Update is called once per frame
    void Update()
    {
        priceProduct.text = (price + (int)(price * priceCof.value)).ToString() + '$';
    }
}
