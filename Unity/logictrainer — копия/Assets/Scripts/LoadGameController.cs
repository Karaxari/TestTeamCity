using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGameController : MonoBehaviour
{
    public void LoadGameMode(string modeSceneName)
    {
        Application.LoadLevel(modeSceneName);
    }
}
