using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace GameGrid
{
    [RequireComponent(typeof(Button))]
    public class TrophyIcon : MonoBehaviour, ISaveable
    {

        [SerializeField]
        protected Queue<Trophy> trophyStored = new Queue<Trophy>();
        [SerializeField]
        protected Text counterHolder;
        [SerializeField]
        protected Sprite icon;
        [SerializeField]
        protected Image iconHolder;


        Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }
        private void Start()
        {
            button.onClick.AddListener(DropStoredTrophy);
        }
        private void OnEnable()
        {
            EventStateHandler.OnSave += Save;
            EventStateHandler.OnLoad += Load;
        }
        //------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------
        public Queue<Trophy> Trophy
        {
            get
            {
                return trophyStored;
            }
            set
            {
                trophyStored = value;
            }
        }
        public string Name
        {
            get;
            set;
        }
        public Sprite Icon
        {
            get
            {
                return iconHolder.sprite;
            }
            set
            {
                iconHolder.sprite = value;
            }
        }
        public int Counter
        {
            get
            {
                return int.Parse(counterHolder.text);
            }
            set
            {
                counterHolder.text = value.ToString();
                if (!button)
                    return;

                if (value <= 0)
                    button.interactable = false;
                else
                    button.interactable = true;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Assigns a trophy inititially to this icon
        /// </summary>
        /// <param name="trophy"></param>
        public void AssignTrophy(Trophy trophy)
        {
            Trophy.Enqueue(trophy);
            Icon = trophy.Icon;
            Name = trophy.Name;
            trophy.StoringIcon = this;
        }
        /// <summary>
        /// Removes a trophy from this icon
        /// </summary>
        public void DropStoredTrophy()
        {
            Trophy trophy = TrophyLibrary.Instance.SpawnTrophy(Trophy.ToArray()[0].Name);
            trophy.StoringIcon = this;
            trophy.StoringIcon.Trophy.Enqueue(trophy);
            trophy.TrophyState = GameGrid.Trophy.State.Held;
            TrophyManager.Instance.CurrentSessionTrophies.Dequeue(); ;
            Destroy(Trophy.ToArray()[0].gameObject);

            //Counter--;
        }
        /// <summary>
        /// Adds a trophy to this icon
        /// </summary>
        /// <param name="trophy"></param>
        public void StoreTrophy(Trophy trophy)
        {
            trophy.StoringIcon = this;
            Trophy.Enqueue(trophy);
            Destroy(trophy.gameObject);
        }
        private void Update()
        {
            CheckOnCount();
            CheckMissingTrophies();
        }
        void CheckOnCount()
        {
            //Counter = Trophy.Count;
            //Trophy = FindObjectsOfType<Trophy>().Where(trophy => trophy.TrophyState == GameGrid.Trophy.State.Stored && trophy.Name == Name).ToList();
            //Counter = Trophy.Count;
        }
        public void Save()
        {
            //ES3.Save(Name + "trophylist", Trophy);
        }
        public void Load()
        {
          // Trophy = ES3.Load(Name + "trophylist") as List<Trophy>;
        }
        void CheckMissingTrophies()
        {
            Trophy.ToList().RemoveAll(x => x == null);
        }
    }
}
