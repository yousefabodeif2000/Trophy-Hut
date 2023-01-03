using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameGrid
{
    public class NullTrophy : MonoBehaviour
    {
        static public NullTrophy Instance;
        static public bool IsAvailable = false;
        public List<MeshRenderer> renderers;
        TrophyManager TrophyManager => TrophyManager.Instance;
        Player Player => Player.Instance;

        private void Awake()
        {
            Instance = this;
        }
        private void Update()
        {
            foreach (MeshRenderer renderer in renderers)
                renderer.enabled = IsAvailable;

            //if (IsAvailable && Player.CurrentState == Player.PlayerState.HoldingTrophy)
            //{
            //    Player.HeldTrophy.gameObject.SetActive(false);
            //    if (Player.hit.transform == null)
            //        transform.position = Camera.main.transform.position + Camera.main.transform.forward * TrophyManager.TrophyOffsetFromCamera;
            //    else
            //        transform.position = Player.hit.point;
            //}
            //else if(!IsAvailable && Player.CurrentState != Player.PlayerState.HoldingTrophy && Player.HeldTrophy)
            //{
            //    Player.HeldTrophy.gameObject.SetActive(true);

            //}
            if (Player.HeldTrophy)
            {
                Player.HeldTrophy.gameObject.SetActive(!IsAvailable);
                Player.HeldTrophy.CanDrop = !IsAvailable;
            }
        }
    }
}
