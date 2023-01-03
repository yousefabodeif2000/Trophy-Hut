using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Not Dimensioanal anymore!
//This script is narrated by Yousef Ahmed
namespace GameGrid
{
    /// <summary>
    /// A class that holds all the tropies as a reference. Will require updates for future trophies.
    /// </summary>
    public class TrophyLibrary : MonoBehaviour
    {
        static public TrophyLibrary Instance;
        [Header("PROTOTYPE")]
        public Trophy tv01lac_0622_2028_first;
        public Trophy tv01lac_0622_2028_second;
        public Trophy tv01soc_0622_2028_first;
        public Trophy tv01soc_0622_2028_second;

        [Header("Library")]
        public Trophy tv01lac_0622_2028_champions;
        public Trophy tv01lac_0622_2028_finalists;
        public Trophy tv01lac_0622_2028_semifinalists;
        public Trophy mh01soc_0922_2028_champions;
        public Trophy mh01soc_0922_2028_finalists;
        public Trophy mh01soc_0922_2028_semifinalists;
        public Trophy mh01soc_0922_2028_charcoal;
        public Trophy mh01soc_0922_2028_champions_ball;
        public Trophy mh01soc_0922_2028_finalists_ball;
        public Trophy mh01soc_0922_2028_semifinalists_ball;
        private void Awake()
        {
            Instance = this;
        }
        public Trophy SpawnTrophy(string trophyName)
        {
            Trophy trophy = new Trophy();
            switch (trophyName)
            {
                case "tv01lac_0622_2028_first":
                    trophy = Instantiate(TrophyLibrary.Instance.tv01lac_0622_2028_first);
                    break;
                case "tv01lac_0622_2028_second":
                    trophy = Instantiate(TrophyLibrary.Instance.tv01lac_0622_2028_second);
                    break;
                case "tv01soc_0622_2028_first":
                    trophy = Instantiate(TrophyLibrary.Instance.tv01soc_0622_2028_first);
                    break;
                case "tv01soc_0622_2028_second":
                    trophy = Instantiate(TrophyLibrary.Instance.tv01soc_0622_2028_second);
                    break;

                //NEW..............................
                case "tv01lac_0622_2028_champions":
                    trophy = Instantiate(TrophyLibrary.Instance.tv01lac_0622_2028_champions);
                    break;
                case "tv01lac_0622_2028_finalists":
                    trophy = Instantiate(TrophyLibrary.Instance.tv01lac_0622_2028_finalists);
                    break;
                case "tv01lac_0622_2028_semifinalists":
                    trophy = Instantiate(TrophyLibrary.Instance.tv01lac_0622_2028_semifinalists);
                    break;
                case "mh01soc_0922_2028_champions":
                    trophy = Instantiate(TrophyLibrary.Instance.mh01soc_0922_2028_champions);
                    break;
                case "mh01soc_0922_2028_finalists":
                    trophy = Instantiate(TrophyLibrary.Instance.mh01soc_0922_2028_finalists);
                    break;
                case "mh01soc_0922_2028_semifinalists":
                    trophy = Instantiate(TrophyLibrary.Instance.mh01soc_0922_2028_semifinalists);
                    break;
                case "mh01soc_0922_2028_charcoal":
                    trophy = Instantiate(TrophyLibrary.Instance.mh01soc_0922_2028_charcoal);
                    break;
                case "mh01soc_0922_2028_champions_ball":
                    trophy = Instantiate(TrophyLibrary.Instance.mh01soc_0922_2028_champions_ball);
                    break;
                case "mh01soc_0922_2028_finalists_ball":
                    trophy = Instantiate(TrophyLibrary.Instance.mh01soc_0922_2028_finalists_ball);
                    break;
                case "mh01soc_0922_2028_semifinalists_ball":
                    trophy = Instantiate(TrophyLibrary.Instance.mh01soc_0922_2028_semifinalists_ball);
                    break;
                default:
                    print("Can't retrieve trophy name, or user isn't assigned a trophy.");
                    UIHandler.Instance.NotifyLoading = " ";
                    UIHandler.Instance.NotifyError = "There was a problem retrieving your trophies. You are not assigned any trophies so please contact a coordinator to assign you your trophies.";
                    break;

            }
            //trophy.EnableTrophy(true);
            TrophyManager.Instance.CurrentSessionTrophies.Enqueue(trophy);
            //TrophyManager.Instance.CurrentSessionTrophies.Remove(trophy);
            return trophy;

        }


    }
}
