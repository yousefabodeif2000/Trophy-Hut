using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameGrid
{
    public class Player : PlayerController, ISaveable
    {
        static public Player Instance;

        NullTrophy nullTrophy;

        Transform Target
        {
            get
            {
                return UIHandler.Instance.playerCamera.transform;
            }
        }
        public enum PlayerState
        {
            NotLookingAtTrophy,
            LookingAtTrophy,
            HoldingTrophy
        }
        private PlayerState state;
        public PlayerState CurrentState
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
            }
        }

        public float lookDistance = 5f;

        static bool storingTrophies;

        RaycastHit hit;
        static public bool HasStoredTrophies
        {
            get
            {
                return storingTrophies;
            }
            set
            {
                storingTrophies = value;
            }
        }
        Trophy heldTrophy;
        /// <summary>
        /// Current held trophy by the player.
        /// </summary>
        [HideInInspector]
        public Trophy HeldTrophy
        {
            get
            {
                return heldTrophy;
            }
            set
            {
                heldTrophy = value;
            }
        }
        TrophyManager TrophyManager => TrophyManager.Instance;

        private void Awake()
        {
            Instance = this;
            EventStateHandler.OnSave += Save;
            EventStateHandler.OnLoad += Load;
            
        }
        public override void Start()
        {
            base.Start();
            nullTrophy = NullTrophy.Instance;
        }
        public override void Update()
        {
            base.Update();

            if (Physics.Raycast(Target.position, Target.forward, out hit, lookDistance))
            {
               // print(" hit is: " + LayerMask.LayerToName(hit.transform.gameObject.layer) + " tm is " + LayerMask.LayerToName(TrophyManager.PlacementMask.value));
                Trophy trophy = hit.transform.GetComponent<Trophy>();
                switch (CurrentState)
                {
                    case PlayerState.LookingAtTrophy:
                        if (trophy)
                        {
                            if (SimpleInput.GetButton("Grab"))
                            {
                                print("grabbed trophy");
                                trophy.Store();
                                trophy.TrophyState = Trophy.State.Stored;

                                CurrentState = PlayerState.NotLookingAtTrophy;
                            }
                        }
                        else
                        {
                            CurrentState = PlayerState.NotLookingAtTrophy;
                        }
                        break;
                    case PlayerState.NotLookingAtTrophy:
                        NullTrophy.IsAvailable = false;
                        if (trophy)
                            CurrentState = PlayerState.LookingAtTrophy;
                        else
                            CurrentState = PlayerState.NotLookingAtTrophy;
                        break;
                    case PlayerState.HoldingTrophy:
                        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Placeable"))
                        {
                            NullTrophy.IsAvailable = false;
                            HeldTrophy.transform.position = hit.point;

                            if (HeldTrophy.CanDrop && SimpleInput.GetButton("Drop"))
                            {
                                print("dropped trophy");
                                HeldTrophy.TrophyState = Trophy.State.Dropped;
                                HeldTrophy = null;
                                NullTrophy.IsAvailable = false;
                                CurrentState = PlayerState.LookingAtTrophy;
                            }
                        }
                        else
                        {
                            NullTrophy.IsAvailable = true;
                            nullTrophy.transform.position = hit.point;
                        }
                        break;
                }
            }
            else
            {

                switch (CurrentState)
                {
                    case PlayerState.HoldingTrophy:
                        NullTrophy.IsAvailable = true;
                        nullTrophy.transform.position = Camera.main.transform.position + Camera.main.transform.forward * TrophyManager.TrophyOffsetFromCamera; ;
                        break;
                }
            }
        }
        public void PlayerCanMove(bool state)
        {
            CanLookAround = state;
        }
        public void Save()
        {
            ES3.Save("playerpos", transform.position);
            print("Player pos saved");
        }

        public void Load()
        {
            if (transform.position != (Vector3)ES3.Load("playerpos"))
            {
                GetComponent<CharacterController>().enabled = false;
                transform.position = (Vector3)ES3.Load("playerpos");
                GetComponent<CharacterController>().enabled = true;
            }
            print("Player pos loaded");
        }
    }
}
