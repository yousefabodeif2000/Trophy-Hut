using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
//Narrated elegantly by Yousef Ahmed
namespace GameGrid
{
    public class UIHandler : MonoBehaviour, ISaveable
    {
        static public UIHandler Instance;

        string message; //notifying message;

        public string NotifyError
        {
            get
            {
                return message;
            }
            set
            {
                message = value;

                if(message == " ")
                {
                    errorNotifierHolder.SetActive(false);
                }
                else
                {
                    errorNotifierHolder.SetActive(true);
                    errorTextHolder.text = message;
                }
            }
        }
        public string NotifyLoading
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
                if (message == " ")
                {
                    loadingNotifierHolder.SetActive(false);
                }
                else
                {
                    loadingNotifierHolder.SetActive(true);
                    loadingTextHolder.text = message;
                }

            }
        }

        [Header("Main Menu References")]
        public Button LoginButton;
        public Button RegisterButton;
        public Button LogoutButton;
        public GameObject MainMenuWindow;

        [Header("Main App References")]
        public Button DropTrophyButton;
        public Button GrabTrophyButton;
        public TMP_Text UserNameDisplayHolder;
        public Transform TrophyIconsHolder;
        public GameObject SavingCog;
        public GameObject LoadingCog;
        public GameObject mainHud;
        public GameObject errorNotifierHolder;
        public TMP_Text errorTextHolder;        
        public GameObject loadingNotifierHolder;
        public TMP_Text loadingTextHolder;
        public GameObject sureWindow; //a window that appears before logging out to make sure you want to log out
        public Camera playerCamera, menuCamera;
        public GameObject Joysticks;
        [Header("Resources")]
        public TrophyIcon TrophyIconPrefab;

        static public bool AssignedCurrentSessionTrophyIcons = false;

        List<TrophyIcon> icons = new List<TrophyIcon>();
        Image grab;
        Image drop;
        Player player => Player.Instance;
        public List<TrophyIcon> Icons
        {
            get
            {
                return icons;
            }
            set
            {
                icons = value;
            }
        }
        private void Awake()
        {
            Instance = this;
            EventStateHandler.OnSave += Save;
            EventStateHandler.OnLoad += Load;
            EventStateHandler.OnReset += Reset;
        }
        private void Start()
        {
            LoginButton.onClick.AddListener(Login);
            RegisterButton.onClick.AddListener(Register);
            LogoutButton.onClick.AddListener(Logout);

            grab = GrabTrophyButton.image;
            drop = DropTrophyButton.image;
        }
        private void Update()
        {
            EnableTrophyButtons();
            if (TrophyIconsHolder.childCount < 1)
                Player.HasStoredTrophies = false;
            else
                Player.HasStoredTrophies = true;
        }
        public void EnableMessages(bool state)
        {
            errorNotifierHolder.SetActive(state);
            loadingNotifierHolder.SetActive(state);
        }
        void Login()
        {
            EventStateHandler.CurrentState = EventStateHandler.AppState.LoggingIn;
            UserAuthenticator.Instance.Login();
        }
        void Register()
        {
            EventStateHandler.CurrentState = EventStateHandler.AppState.Registering;
        }
        void Logout()
        {
            EventStateHandler.CurrentState = EventStateHandler.AppState.LoggingOut;
            MainMenuWindow.SetActive(true);
        }

       public void ShowMainMenu(bool state)
        {
            MainMenuWindow.SetActive(state);
        }
        /// <summary>
        /// Enables trophy buttons based on the current state
        /// </summary>
        /// <param name="state">State of the button</param>
        void EnableTrophyButtons()
        {
            if (player.CurrentState == Player.PlayerState.LookingAtTrophy)
            {
                grab.enabled = true;
                drop.enabled = false;
            }
            if (player.CurrentState == Player.PlayerState.HoldingTrophy && player.HeldTrophy.CanDrop)
            {
                grab.enabled = false;
                drop.enabled = true;

            }
            if (player.CurrentState == Player.PlayerState.HoldingTrophy && !player.HeldTrophy.CanDrop || player.CurrentState == Player.PlayerState.NotLookingAtTrophy)
            {
                grab.enabled = false;
                drop.enabled = false;
            }
        }

        public void Initialize()
        {
            StartCoroutine(InitializingAppHUD());
        }
        public void AssignIcon(Trophy _trophy)
        {

            TrophyIcon existingIcon = Icons.Where(trophy => trophy.Name == _trophy.Name).FirstOrDefault();
            if (existingIcon)
            {

                //for (int i = 0; i < Icons.Count; i++)
                //{
                //    if (Icons[i].Name.Contains(_trophy.name))
                //        existingIcon = Icons[i];
                //}
                //int existingCount = existingIcon.Counter;
                existingIcon.Counter++;
                //if (existingIcon.Counter > existingCount + 1)
                //    existingIcon.Counter--;

                //if (existingIcon.Trophy.Count < 1)
                existingIcon.Trophy.Enqueue(_trophy);
                print("name exist");
            }
            else
            {
                print("name didn't exist");
                AddIcon(_trophy);
            }
            /*
        foreach(TrophyIcon icon in Icons.ToArray())
        {
            if (icon.Name == _trophy.Name)
            {
                icon.StoreTrophy(_trophy);
                print("added counter");
                break;
            }
            else
            {
                AddIcon(_trophy);
                print("added icon");
            }
        }*/



        }
        void AddIcon(Trophy trophy)
        {
            TrophyIcon icon = Instantiate(TrophyIconPrefab, TrophyIconsHolder);
            icon.AssignTrophy(trophy);
            Icons.Add(icon);
        }
        IEnumerator InitializingAppHUD()
        {
            while (!TrophyManager.CurrentSessionTrophiesInitialized)
            {
                print("Initializing current trophy in the session, please wait...");
                yield return null;
            }
            print("Initializing UI and icons..");

            UserNameDisplayHolder.text = UserAuthenticator.CurrentUser.Name;

            foreach (Trophy trophy in TrophyManager.Instance.CurrentSessionTrophies)
            {
                AssignIcon(trophy);
            }
            AssignedCurrentSessionTrophyIcons = true;
            EventStateHandler.CurrentState = EventStateHandler.AppState.Normal;
            LoadingCog.SetActive(false);
        }
        IEnumerator Saving()
        {
            SavingCog.SetActive(true);
            ES3.Save("mainmenu window", MainMenuWindow.activeSelf);
            ES3.Save("username holder", UserNameDisplayHolder.text);
            yield return new WaitForSeconds(3f);
            SavingCog.SetActive(false);
        }
        IEnumerator Loading()
        {
            LoadingCog.SetActive(true);
            MainMenuWindow.SetActive(ES3.Load("mainmenu window", MainMenuWindow.activeSelf));
            UserNameDisplayHolder.text = (string)ES3.Load("username holder");
            while (!TrophyManager.CurrentSessionTrophiesInitialized)
                yield return null;
            //Icons.Clear();
            foreach (Trophy trophy in TrophyManager.Instance.CurrentSessionTrophies)
            {
                 AssignIcon(trophy);
            }
            AssignedCurrentSessionTrophyIcons = true;
            EventStateHandler.CurrentState = EventStateHandler.AppState.Normal;
            LoadingCog.SetActive(false);
        }
        public void Save()
        {
            StartCoroutine(Saving());
        }

        public void Load()
        {
            StartCoroutine(Loading());
        }
        private void Reset()
        {
            foreach(TrophyIcon i in Icons)
            {
                Destroy(i.gameObject);

            }
            Icons.Clear();
            AssignedCurrentSessionTrophyIcons = false;
        }
        public void ShowHUD(bool state)
        {
            mainHud.SetActive(state);
        }
        public void ShowJoystick(bool state)
        {
            Joysticks.SetActive(state);
        }
        public void QuitAPP()
        {
            Application.Quit();
        }
    }

}
