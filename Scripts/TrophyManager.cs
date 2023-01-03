using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameGrid
{
    public class TrophyManager : MonoBehaviour, ISaveable
    {
        static public TrophyManager Instance;

        Queue<Trophy> trophies = new Queue<Trophy>();
        /// <summary>
        /// A list that holds all the current trophies in the user session.
        /// </summary>
        public Queue<Trophy> CurrentSessionTrophies
        {
            get
            {
                return trophies;
            }
            set
            {
                trophies = value;
            }
        }
        //public List<Trophy> SpawnedTrophies = new List<Trophy>();
        private void Awake()
        {
            Instance = this;
            EventStateHandler.OnSave += Save;
            EventStateHandler.OnLoad += Load;
            EventStateHandler.OnReset += Reset;
        }
        /// <summary>
        /// The number of actual trophies the user have in the current session.
        /// </summary>
        public int SessionTrophyTypesFlagsCount
        {
            get
            {
                return trophyTypesFlagCount;
            }
            set
            {
                trophyTypesFlagCount = value;
            }
        }

        static public int trophySaveID = 0;

        int trophyTypesFlagCount = 0;

        static public bool CurrentSessionTrophiesInitialized = false;

        [Header("Settings")]
        public float TrophyOffsetFromCamera = 2f;
        /// <summary>
        /// The layer of the game object where a trophy can be placed on.
        /// </summary>
        public LayerMask PlacementMask;

        /// <summary>
        /// Initializes trophies based on the user registered trophies, calling them from the trophy library.
        /// </summary>
        public void InitializeTrophies()
        {
            //StartCoroutine(InitializingTrophies());
        }
        //Note: update this with the new AddTrophy in TrophyLib later
        /*
        IEnumerator InitializingTrophies()
        {
            while (!UserAuthenticator.TrophiesDataRetrieved)
            {
                print("Retrieving Trophy data from the authenticator, please wait...");
                yield return null;
            }
            User User = UserAuthenticator.CurrentUser;
            print("Initializing Trophies, User trophy flags are: " + User.UserTrophyType);
            if (User.UserTrophyType.HasFlag(User.TrophyType.LacrosseChampionsFirst))
            {
                Trophy trophy = Instantiate(TrophyLibrary.Instance.LACROSSE_CHAMPIONS_FIRST);
                trophy.EnableTrophy(false);
                CurrentSessionTrophies.Add(trophy);
                //User.UserTrophyType &= ~User.TrophyType.LacrosseChampionsFirst;
                print("Added a Lacrosse Champions First trophy.");
            }
            if (User.UserTrophyType.HasFlag(User.TrophyType.LacrosseChampionsSecond))
            {
                Trophy trophy = Instantiate(TrophyLibrary.Instance.LACROSSE_CHAMPIONS_SECOND);
                trophy.EnableTrophy(false);
                CurrentSessionTrophies.Add(trophy);
                //User.UserTrophyType &= ~User.TrophyType.LacrosseChampionsSecond;
                print("Added a Lacrosse Champions Second trophy.");
            }
            if (User.UserTrophyType.HasFlag(User.TrophyType.SoccerFinalistsFirst))
            {
                Trophy trophy = Instantiate(TrophyLibrary.Instance.SOCCER_FINALISTS_FIRST);
                trophy.EnableTrophy(false);
                CurrentSessionTrophies.Add(trophy);
                //User.UserTrophyType &= ~User.TrophyType.SoccerFinalistsFirst;
                print("Added a Soccer Finalists First trophy.");
            }
            if (User.UserTrophyType.HasFlag(User.TrophyType.SoccerFinalistsSecond))
            {
                Trophy trophy = Instantiate(TrophyLibrary.Instance.SOCCER_FINALISTS_SECOND);
                trophy.EnableTrophy(false);
                CurrentSessionTrophies.Add(trophy);
                //User.UserTrophyType &= ~User.TrophyType.SoccerFinalistsSecond;
                print("Added a Soccer Finalists Second trophy.");
            }
            if (User.UserTrophyType.HasFlag(User.TrophyType.None) && User.UserType == User.Type.Coordinator)
            {
                //Show that there's no trophy for this user message.
                print("No trophy for this user");
            }
            else
            {
                Debug.LogError("Can't get trophies data from the user to spawn them!");
            }


        }
        */
        public void AddATrophy(string type)
        {
            Trophy trophy = new Trophy();
            switch (type)
            {
                case "Trophies/tv01lac_0622_2028_first":
                    trophy = Instantiate(TrophyLibrary.Instance.tv01lac_0622_2028_first);
                    break;
                case "Trophies/tv01lac_0622_2028_second":
                    trophy = Instantiate(TrophyLibrary.Instance.tv01lac_0622_2028_second);
                    break;
                case "Trophies/tv01soc_0622_2028_first":
                    trophy = Instantiate(TrophyLibrary.Instance.tv01soc_0622_2028_first);
                    break;
                case "Trophies/tv01soc_0622_2028_second":
                    trophy = Instantiate(TrophyLibrary.Instance.tv01soc_0622_2028_second);
                    break;

                    //NEW..............................
                case "Trophies/tv01lac_0622_2028_champions":
                    trophy = Instantiate(TrophyLibrary.Instance.tv01lac_0622_2028_champions);
                    break;
                case "Trophies/tv01lac_0622_2028_finalists":
                    trophy = Instantiate(TrophyLibrary.Instance.tv01lac_0622_2028_finalists);
                    break;
                case "Trophies/tv01lac_0622_2028_semifinalists":
                    trophy = Instantiate(TrophyLibrary.Instance.tv01lac_0622_2028_semifinalists);
                    break;
                case "Trophies/mh01soc_0922_2028_champions":
                    trophy = Instantiate(TrophyLibrary.Instance.mh01soc_0922_2028_champions);
                    break;
                case "Trophies/mh01soc_0922_2028_finalists":
                    trophy = Instantiate(TrophyLibrary.Instance.mh01soc_0922_2028_finalists);
                    break;
                case "Trophies/mh01soc_0922_2028_semifinalists":
                    trophy = Instantiate(TrophyLibrary.Instance.mh01soc_0922_2028_semifinalists);
                    break;
                case "Trophies/mh01soc_0922_2028_charcoal":
                    trophy = Instantiate(TrophyLibrary.Instance.mh01soc_0922_2028_charcoal);
                    break;
                case "Trophies/mh01soc_0922_2028_champions_ball":
                    trophy = Instantiate(TrophyLibrary.Instance.mh01soc_0922_2028_champions_ball);
                    break;
                case "Trophies/mh01soc_0922_2028_finalists_ball":
                    trophy = Instantiate(TrophyLibrary.Instance.mh01soc_0922_2028_finalists_ball);
                    break;
                case "Trophies/mh01soc_0922_2028_semifinalists_ball":
                    trophy = Instantiate(TrophyLibrary.Instance.mh01soc_0922_2028_semifinalists_ball);
                    break;
                default:
                    print("Can't retrieve trophy name, or user isn't assigned a trophy.");
                    UIHandler.Instance.NotifyLoading = " ";
                    UIHandler.Instance.NotifyError = "There was a problem retrieving your trophies. You are not assigned any trophies so please contact a coordinator to assign you your trophies.";
                    break;

            }
            trophy.EnableTrophy(false);
            CurrentSessionTrophies.Enqueue(trophy);
        }
        IEnumerator Loading()
        {

            print("Loading current session trophies");
            List<GameObject> sessionObjs = ES3.Load("Current Session Trophies", new List<GameObject>());

            foreach (GameObject obj in sessionObjs)
            {
                Trophy trophy = obj.GetComponent<Trophy>();
                CurrentSessionTrophies.Enqueue(trophy);
            }
            CurrentSessionTrophiesInitialized = true;
            print("Current session trophies initialized successfully");
            while (!UIHandler.AssignedCurrentSessionTrophyIcons)
            {
                print("Waitng for UIHandler to assign icons");
                yield return null;
            }
            foreach (Trophy trophy in CurrentSessionTrophies)
            {
                trophy.Load();
            }
        }
        public void Save()
        {
            List<GameObject> sessionObjs = new List<GameObject>();

            foreach(var trophy in CurrentSessionTrophies)
            {
                if (trophy != null)
                    sessionObjs.Add(trophy.gameObject);
            }
            ES3.Save("Current Session Trophies", sessionObjs);
            print("Current trophies in session saved");
        }

        public void Load()
        {
            StartCoroutine(Loading());
        }
        private void Reset()
        {
            foreach(Trophy t in CurrentSessionTrophies)
            {
                Destroy(t.gameObject);
            }
            CurrentSessionTrophies.Clear();
            CurrentSessionTrophiesInitialized = false;
        }
    }

}
