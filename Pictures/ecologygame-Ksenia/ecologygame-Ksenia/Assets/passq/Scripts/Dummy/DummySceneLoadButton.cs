using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DummySceneLoadButton : MonoBehaviour
{
    [SerializeField] private int sceneIndex;

    public void OnClick()
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
