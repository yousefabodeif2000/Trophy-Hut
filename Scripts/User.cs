using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GameGrid
{
    public class User
    {
        public string Name;
        public string Email;
        public enum Type
        {
            User = 0,
            Coordinator = 1
        }
        public Type UserType;

        [Flags]
        public enum TrophyType
        {
            None = 1,
            LacrosseChampionsFirst = 2,
            LacrosseChampionsSecond = 3,
            SoccerFinalistsFirst = 4,
            SoccerFinalistsSecond = 5
        }

        public TrophyType UserTrophyType;

        /// <summary>
        /// Creates a blank user
        /// </summary>
        public User()
        {
            Name = "New User";
            Email = "newuser@newuser.newuser";
            UserType = Type.User;
            UserTrophyType = TrophyType.None;
        }
        /// <summary>
        /// Creates a user
        /// </summary>
        public User(string name, string email)
        {
            Name = name;
            Email = email;
            UserType = Type.User;
            UserTrophyType = TrophyType.None;
        }
        /// <summary>
        /// Creates a user
        /// </summary>
        public User(string name, string email, Type type, TrophyType trophyType)
        {
            Name = name;
            Email = email;
            UserType = type;
            UserTrophyType = trophyType;
        }
    }
}
