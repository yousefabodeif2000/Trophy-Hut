using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace GameGrid
{
    [RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
    public class Trophy : MonoBehaviour, ISaveable
    {
        public int ID;
        public string Name = "New Trophy";
        public Sprite Icon;
        [SerializeField]
        private bool IsEnabled;

        public bool IsMaster;
        public List<Renderer> renderers
        {
            get
            {
                return GetComponentsInChildren<Renderer>().ToList();
            }
        }
        /// <summary>
        /// Icon that stores this trophy.
        /// </summary>
       // [HideInInspector]
        public TrophyIcon StoringIcon;
        public enum State
        {
            Stored, //Stored in an icon
            Held, //Held to be placed somewhere.
            Dropped //Placed on ground
        }
        State state;

        bool isHeld, canDrop;

        public Collider collider;

        private void Awake()
        {
            EventStateHandler.OnSave += Save;
            EventStateHandler.OnLoad += Load;
        }
        public bool CanDrop
        {
            get
            {
                return canDrop;
            }
            set
            {
                canDrop = value;
            }
        }
        TrophyManager TrophyManager => TrophyManager.Instance;
        float offset
        {
            get
            {
                return TrophyManager.Instance.TrophyOffsetFromCamera;
            }
        }
        public State TrophyState
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
                //if (StoringIcon == null)
                //    UIHandler.Instance.AssignIcon(this);
                switch (value)
                {
                    case State.Stored:
                        SoundManager.Instance.PlaySoundFX(SoundManager.Instance.Pickup);
                        if (EventStateHandler.CurrentState != EventStateHandler.AppState.Loading)
                            EventStateHandler.CurrentState = EventStateHandler.AppState.Saving;
                        break;

                    case State.Held:
                        UIHandler.Instance.ShowHUD(false);
                        UIHandler.Instance.ShowJoystick(true);
                        isHeld = true;
                        break;

                    case State.Dropped:
                        Drop();
                        SoundManager.Instance.PlaySoundFX(SoundManager.Instance.Drop);
                        if (EventStateHandler.CurrentState != EventStateHandler.AppState.Loading)
                            EventStateHandler.CurrentState = EventStateHandler.AppState.Saving;
                        break;
                }


            }
        }

        public enum TrophyTypes
        {

        }

        private void Start()
        {
          //  TrophyState = TrophyState;
        }
        private void Update()
        {
            //if (StoringIcon == null)
              //  StoringIcon = UIHandler.Instance.Icons.Where(icon => icon.Name == Name).FirstOrDefault();
        }
        private void LateUpdate()
        {
            if (isHeld)
            {
                Hold();
            }
        }

        public void Store()
        {
            if (StoringIcon == null)
                StoringIcon = UIHandler.Instance.Icons.Where(icon => icon.Name == Name).FirstOrDefault();

            if (EventStateHandler.CurrentState != EventStateHandler.AppState.Loading)
            {
                UIHandler.Instance.AssignIcon(this);
            }
            TrophyManager.Instance.CurrentSessionTrophies.Dequeue();
            EnableTrophy(false);

        }
        private void OnDestroy()
        {
            //TrophyManager.Instance.CurrentSessionTrophies.Remove(this);
        }
        private void OnEnable()
        {
            print(Name + " Instance ID is " + gameObject.GetInstanceID());
            if (StoringIcon == null)
                StoringIcon = UIHandler.Instance.Icons.Where(icon => icon.Name == Name).FirstOrDefault();
        }
        void Hold()
        {
            collider.isTrigger = true;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            Player.Instance.CurrentState = Player.PlayerState.HoldingTrophy;
            if (!Player.Instance.HeldTrophy)
                Player.Instance.HeldTrophy = this;

        }
        void Drop()
        {
            if (isHeld)
            {
                UIHandler.Instance.ShowHUD(true);
                isHeld = false;
            }
            collider.isTrigger = false;
            gameObject.layer = LayerMask.NameToLayer("Default");
            transform.position = transform.position;
            StoringIcon.Trophy.Dequeue();
            StoringIcon.Counter--;
        }
        public void EnableTrophy(bool state)
        {
            IsEnabled = state;
            foreach(MeshRenderer renderer in renderers)
            {
                renderer.enabled = state;
            }
            collider.enabled = state;
        }
        public void CheckIconStateOnLoad()
        {
            if (StoringIcon == null)
                StoringIcon = UIHandler.Instance.Icons.Where(icon => icon.Name == Name).FirstOrDefault();
            if (EventStateHandler.CurrentState == EventStateHandler.AppState.Loading && TrophyState == State.Dropped)
            {
                print("Checked trophy icon state");
                if (!StoringIcon)
                    return;

                StoringIcon.Trophy.Dequeue();
                StoringIcon.Counter--;
            }
        }
        public void Save()
        {
            //ES3.Save(Name + "obj", gameObject);
            //ES3.Save(Name + "pos", transform.position);
            //ES3.Save(Name + "state", TrophyState);
            //ES3.Save(Name + "master", IsMaster);
            //Debug.Log("Saving " + Name + "state" + TrophyState);
        }

        public void Load()
        {
            //print("loading trophy data");
            //ES3.Load(Name + "obj");
            //transform.position = ES3.Load(Name + "pos", transform.position);
            //StoringIcon = UIHandler.Instance.Icons.Where(icon => icon.Name == Name)
            //TrophyState = (State)ES3.Load(Name + "state");
            //Debug.Log("Loading " + Name + "state" + (State)ES3.Load(Name + "state"));
            // IsMaster = (bool)(ES3.Load(Name + "master"));
            //CheckIconStateOnLoad();
            if (IsRenderersActive())
                TrophyState = State.Dropped;


        }
        bool IsRenderersActive()
        {
            bool active = true;
            foreach (MeshRenderer renderer in renderers)
            {
                if (renderer.enabled == false)
                    active = false;
            }
            return active;
        }
    }
}
