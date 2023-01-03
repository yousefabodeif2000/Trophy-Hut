using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameGrid
{
    public class EventStateHandler : MonoBehaviour
    {
        public enum AppState
        {
            Registering = 0,
            LoggingIn = 1,
            LoggingOut = 2,
            Initialize = 3,
            Saving = 4,
            Loading = 5,
            Normal

        }
        static private AppState state;
        static public Action OnSave;
        static public Action OnLoad;
        static public Action OnReset;
        static public bool InitialStateRetrieved = false;
        private void Awake()
        {
            OnReset += Reset;
        }
        private void Start()
        {
            if (ES3.KeyExists("LoggedIn"))
            {
                print("User is logged in!");
                UserAuthenticator.IsLoggedIn = true;
                CurrentState = AppState.Loading;
                InitialStateRetrieved = true;
            }
            else
            {
                print("User is not logged in, please login!");
                UserAuthenticator.Instance.Initialize();
                InitialStateRetrieved = true;
            }
        }
        static public AppState CurrentState
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
                print("State switched to " + CurrentState);
                switch (value)
                {
                    case AppState.Registering:
                        UserAuthenticator.Instance.Register();
                        UIHandler.Instance.ShowJoystick(false);
                        break;
                    case AppState.LoggingIn:
                        //UserAuthenticator.Instance.Login();
                        UIHandler.Instance.playerCamera.enabled = false;
                        UIHandler.Instance.menuCamera.enabled = true;
                        UIHandler.Instance.ShowJoystick(false);
                        UIHandler.Instance.ShowHUD(false);
                        UIHandler.Instance.EnableMessages(false);
                        UIHandler.Instance.NotifyError = " ";
                        UIHandler.Instance.NotifyLoading = " ";
                        break;
                    case AppState.LoggingOut:
                        OnReset?.Invoke();
                        UserAuthenticator.Instance.Initialize();
                        UserAuthenticator.Instance.Logout();
                        //UIHandler.Instance.EnableMessages(false);
                        break;
                    case AppState.Initialize:
                        TrophyManager.Instance.InitializeTrophies();
                        UIHandler.Instance.Initialize();
                        break;
                    case AppState.Saving:
                        OnSave?.Invoke();
                        break;
                    case AppState.Loading:
                        OnLoad?.Invoke();
                        break;
                    case AppState.Normal:
                        UIHandler.Instance.playerCamera.enabled = true;
                        UIHandler.Instance.menuCamera.enabled = false;
                        UIHandler.Instance.ShowHUD(true);
                        UIHandler.Instance.ShowJoystick(true);
                        UIHandler.Instance.ShowMainMenu(false);
                        break;
                }
            }
        }
        private void Reset()
        {
            InitialStateRetrieved = true;   
        }
    }
}