using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickPoint : MonoBehaviour, IPointerClickHandler
{
    MeshGeneration mesh;
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        string name = this.gameObject.name;

        name = name.Replace("Point_", "");
        Debug.Log("Click " + name);

        int num = Int32.Parse(name);

        int n1 = num / mesh.widthMesh;
        int n2 = num % mesh.widthMesh;

        mesh.checkResult(n1, n2);
        
        //throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        mesh = FindObjectOfType<MeshGeneration>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
