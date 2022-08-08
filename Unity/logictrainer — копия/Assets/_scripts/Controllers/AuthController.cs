using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum AuthErrors
{
    //edit
}
public class AuthController : MonoBehaviour
{
    [SerializeField] private string NextSceneToLoad = null;
    [SerializeField] private string PlatformRefgistrationScene = "PlatformRegistration";
    [SerializeField] private GameObject banner;

    [Header("UI panels")]
    public GameObject AuthPanel;
    public GameObject ResetPasswordPanel;
    public GameObject ResetPasswordSuccess;

    [Header("UI Elements")]
    public InputField emailInput;
    public InputField passwordInput;
    public InputField resetPasswordEmailInput;

    private void Start()
    {
        // Отключение автоматической автозагрузки
        PlayerPrefs.DeleteAll();

        //If Firebase Emulator is acitve, do nothing, because this object authentificates in system
        if (FirebaseProjectConfigurations.PROJECT_BUILD == ProjectBuildType.Emulator)
            return;

        if (FirebaseProjectConfigurations.PROJECT_BUILD == ProjectBuildType.Release)
        {
            InitView();
        }
            
        DataController dataController = FindObjectOfType<DataController>();
        if (dataController != null)
            Destroy(dataController.gameObject);


        if (FirebaseManager.Instance.Auth.HasActiveSession())
        {
            string refreshToken = FirebaseManager.Instance.Auth.GetRefreshToken();
            FirebaseManager.Instance.Auth.RefreshToken(refreshToken, ()=>
            {

                GoToNextScene();
            }, ()=>
            {
                HideBanner();
            });
        }
        else
        {
            HideBanner();
        }
    }

    private void InitView()
    {
        ShowBanner();
        EnableAndClearAuthUI();

        AuthPanel.SetActive(true);
        ResetPasswordPanel.SetActive(false);
        ResetPasswordSuccess.SetActive(false);
    }
    
    //UI Handlers
    public void OnSignInClicked()
    {
        if (string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(passwordInput.text))
            return;

        AuthPanel.transform.FindDeepChild("ErrorMessage").GetComponent<Text>().text = "";

        DisableMenuButtons(AuthPanel);

        FirebaseManager.Instance.Auth.SignInEmailPassword(emailInput.text, passwordInput.text, (response)=>
        {
            FirebaseManager.Instance.Auth.SetIdToken(response.idToken);
            FirebaseManager.Instance.Auth.SetLocalId(response.localId);
            FirebaseManager.Instance.Auth.SetRefreshToken(response.refreshToken);

            GoToNextScene();
        }, (exception)=>
        {
            Debug.Log($"Failed to sign in. Message: {exception.Message}");

            AuthPanel.transform.FindDeepChild("ErrorMessage").GetComponent<Text>().text = "Не удалось войти. Проверьте данные";
            EnableMenuButtons(AuthPanel);
        });
    }
    public void OnSignUpClicked()
    {
        GoToPlatformRegisterScene();
    }
    public void OnResetPasswordClicked()
    {
        if (string.IsNullOrEmpty(resetPasswordEmailInput.text))
            return;

        ResetPasswordPanel.transform.FindDeepChild("ErrorMessage").GetComponent<Text>().text = "";

        DisableMenuButtons(ResetPasswordPanel);

        FirebaseManager.Instance.Auth.SendPasswordResetEmail(resetPasswordEmailInput.text, (success) =>
        {
            if (success)
            {
                ResetPasswordPanel.SetActive(false);
                ResetPasswordSuccess.SetActive(true);

                EnableMenuButtons(ResetPasswordPanel);
            }
            else
            {
                EnableMenuButtons(ResetPasswordPanel);

                ResetPasswordPanel.transform.FindDeepChild("ErrorMessage").GetComponent<Text>().text =
                     $"Произошла ошибка. Проверьте правильность email адреса";
            }
        });
    }

    private void GoToNextScene()
    {
        if(string.IsNullOrEmpty(NextSceneToLoad))
        {
            Debug.Log("Cannot load next scene. NextSceneToLoad is null or empty");
            return;
        }

        SceneManager.LoadScene(NextSceneToLoad);
    }
    private void GoToPlatformRegisterScene()
    {
        SceneManager.LoadScene(PlatformRefgistrationScene);
    }

    //UI
    private void EnableAndClearAuthUI()
    {
        Debug.Log("Enabling auth ui...");
        AuthPanel.transform.FindDeepChild("ErrorMessage").GetComponent<Text>().text = "";

        AuthPanel.SetActive(true);
        banner.SetActive(false);

        emailInput.text = "";
        passwordInput.text = "";
        resetPasswordEmailInput.text = "";
    }
    private void ShowBanner()
    {
        banner.SetActive(true);
    }
    private void HideBanner()
    {
        banner.SetActive(false);
    }
    private void EnableMenuButtons(GameObject panel)
    {
        Button[] btns = panel.transform.FindDeepChild("Menu").GetComponentsInChildren<Button>();
        foreach (var btn in btns)
        {
            btn.interactable = true;
        }
    }
    private void DisableMenuButtons(GameObject panel)
    {
        Button[] btns = panel.transform.FindDeepChild("Menu").GetComponentsInChildren<Button>();
        foreach (var btn in btns)
        {
            btn.interactable = false;
        }
    }
}


