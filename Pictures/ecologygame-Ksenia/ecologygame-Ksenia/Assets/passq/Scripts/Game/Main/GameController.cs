using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [SerializeField] private GameUI UI;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Invoke("Initialize", Time.deltaTime);
    }

    private void Initialize()
    {
        DialogueController.Instance.Initialize();
        DialogueController.Instance.StartDialogue();
        DialogueController.Instance.OnDialogueEnded.AddListener(OnDialogueEnded);
    }

    private void OnDialogueEnded(Dialogue dialogue)
    {
        EndGame();
    }

    private void EndGame()
    {
        UI.Show();
    }
}
