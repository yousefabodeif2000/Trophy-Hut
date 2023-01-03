using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement;
using Firebase.Firestore;
using System.Linq;
using Firebase.Extensions;
using System.Collections.Generic;

//Narrated by Yousef Ahmed
namespace GameGrid
{
    public class UserAuthenticator : MonoBehaviour, ISaveable
    {

        static public UserAuthenticator Instance;
        /// <summary>
        /// our current user.
        /// </summary>
        static User user;

        /// <summary>
        /// our current user property
        /// </summary>
        static public User CurrentUser
        {
            get
            {
                return user;
            }
            set
            {

            }
        }
        string userDocReferenceID = "";

        public string UserDocumentReferenceID
        {
            get
            {
                return userDocReferenceID;
            }
            set
            {
                userDocReferenceID = value;
            }
        }
        #region Firebase Vars
        //Firebase variables
        [Header("Firebase Settings")]
        public DependencyStatus dependencyStatus;
        public FirebaseAuth auth;
        public FirebaseUser FBUser;
        public FirebaseFirestore database;

        //Login variables
        [Header("Login References")]
        public TMP_InputField emailLoginField;
        public TMP_InputField passwordLoginField;
        public TMP_Text warningLoginText;
        public TMP_Text confirmLoginText;

        //Register variables
        [Header("Register References")]
        public TMP_InputField usernameRegisterField;
        public TMP_InputField emailRegisterField;
        public TMP_InputField passwordRegisterField;
        public TMP_InputField passwordRegisterVerifyField;
        public TMP_Text warningRegisterText;

        public static bool IsLoggedIn = false;
        public static bool TrophiesDataRetrieved = false;

        CollectionReference collection;
        #endregion


        private void Awake()
        {
            Instance = this;
            FirebaseApp.Create();
            print("Firebase app created");
            //Check that all of the necessary dependencies for Firebase are present on the system
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.Log("Could not resolve all Firebase dependencies: " + dependencyStatus);
                UIHandler.Instance.NotifyLoading = " ";
                UIHandler.Instance.NotifyError = "There's a problem connecting to the server. Server status: " + dependencyStatus;
                }
            });
        }
        private void OnEnable()
        {
            EventStateHandler.OnSave += Save;
            EventStateHandler.OnReset += Reset;
        }
        private void Start()
        {

        }
        #region Firebase Methods
        private void InitializeFirebase()
        {
            //Set the authentication instance object
            auth = FirebaseAuth.DefaultInstance;
            database = FirebaseFirestore.DefaultInstance;
        }
        public void Initialize()
        {
            StartCoroutine(Initializer());
        }
        private IEnumerator LoggingIn(string _email, string _password)
        {
            //Call the Firebase auth signin function passing the email and password
            var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            //yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);
            while (!LoginTask.IsCompleted)
            {
                UIHandler.Instance.NotifyLoading = "Logging in...";
                yield return null;
            }
            if (LoginTask.Exception != null)
            {
                //If there are errors handle them
               // UIManager.Instance.HideLoading();
                Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
                FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                string message = "Login Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        yield return null;
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WrongPassword:
                        message = "Wrong Password";
                        break;
                    case AuthError.InvalidEmail:
                        message = "Invalid Email";
                        break;
                    case AuthError.UserNotFound:
                        message = "Account does not exist";
                        break;
                }
                //warningLoginText.text = message;
                UIHandler.Instance.NotifyLoading = " ";
                UIHandler.Instance.NotifyError = message;
            }
            else
            {
                //User is now logged in
                //Now get the result
                FBUser = LoginTask.Result;

                Debug.LogFormat("User signed in successfully: {0} ({1})", FBUser.DisplayName, FBUser.Email);
                UIHandler.Instance.NotifyError = " ";
                UIHandler.Instance.NotifyLoading = "Welcome, " + FBUser.DisplayName + "!";

                IsLoggedIn = true;


            }
        }

        private IEnumerator Registering(string _email, string _password, string _username)
        {
            if (_username == "")
            {
                //If the username field is blank show a warning
                //UIHandler.Instance.NotifyLoading = " ";
                //UIHandler.Instance.NotifyError = "Missing Username";
                yield return null;
            }
            else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
            {
                //If the password does not match show a warning
                UIHandler.Instance.NotifyLoading = " ";
                UIHandler.Instance.NotifyError = "Passwords Does Not Match!";
            }
            else
            {
                //Call the Firebase auth signin function passing the email and password
                var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
                //Wait until the task completes
                // yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);
                while (!RegisterTask.IsCompleted)
                {
                    //UIManager.Instance.ShowLoading();
                    yield return null;
                }
                if (RegisterTask.Exception != null)
                {
                    //If there are errors handle them
                    Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                    FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                    AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                   //UIManager.Instance.HideLoading();
                    string message = "Register Failed!";
                    switch (errorCode)
                    {
                        case AuthError.MissingEmail:
                            yield return null;
                            break;
                        case AuthError.MissingPassword:
                            message = "Missing Password";
                            break;
                        case AuthError.WeakPassword:
                            message = "Weak Password";
                            break;
                        case AuthError.EmailAlreadyInUse:
                            message = "Email Already In Use";
                            break;
                    }
                    UIHandler.Instance.NotifyLoading = " ";
                    UIHandler.Instance.NotifyError = message;
                }
                else
                {
                    //User has now been created
                    //Now get the result
                    FBUser = RegisterTask.Result;

                    if (FBUser != null)
                    {
                        //Create a user profile and set the username
                        UserProfile profile = new UserProfile { DisplayName = _username };

                        //Call the Firebase auth update user profile function passing the profile with the username
                        var ProfileTask = FBUser.UpdateUserProfileAsync(profile);
                        //Wait until the task completes
                        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                        if (ProfileTask.Exception != null)
                        {
                            //If there are errors handle them
                            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                            FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                            UIHandler.Instance.NotifyLoading = " ";
                            UIHandler.Instance.NotifyError = "Username Set Failed!";
                        }
                        else
                        {
                            //Username is now set
                            //Now return to login screen
                            confirmLoginText.text = "Thanks for Registering!";
                            UIHandler.Instance.NotifyLoading = "Thanks for Registering!";
                            UIHandler.Instance.NotifyError = " ";

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves user data including his type and trophies he has.
        /// </summary>
        /// <returns></returns>
        private IEnumerator RetrieveUserDoccumentReferenceID()
        {
            while (!IsLoggedIn)
            {
                print("User still didn't login");
                yield return null;
            }
            UIHandler.Instance.NotifyLoading = "Please wait while we retrieve your data...";
            user = new User(FBUser.DisplayName, FBUser.Email);
            #region Getting Doc Reference

            collection = database.Collection("Users");
            Query query = collection.WhereEqualTo("email", FBUser.Email);
            query.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
            {
                if (querySnapshotTask.IsFaulted)
                {
                    print("Error trying to retrieve user document reference id, " + querySnapshotTask.Exception.Message);

                }
                else if (querySnapshotTask.IsCompleted)
                {
                    if (querySnapshotTask.Result.Documents.Count() < 1)
                    {
                        print("No data found for this user");
                        UIHandler.Instance.NotifyLoading = " ";
                        UIHandler.Instance.NotifyError = "No data found for this user! Please contact an adminstrator";
                    }
                    else
                    {
                        DocumentSnapshot snapshot = querySnapshotTask.Result.Documents.FirstOrDefault();
                        UserDocumentReferenceID = snapshot.Id;

                    }
                }
            });
            #endregion



            EventStateHandler.CurrentState = EventStateHandler.AppState.Initialize;
            yield return null;
        }
        private IEnumerator RetrieveUserData()
        {
            #region Getting Data
            while (UserDocumentReferenceID == "")
            {
                print("retrieving user doc ref id");
                yield return null;
            }
            print("Parsing...");
            UIHandler.Instance.NotifyLoading = "Please wait while we parse your data...";
            List<object> retrievedTrophyNames = new List<object>();
            //dictionary containing all fields retrieved from that document from firebase.
            Dictionary<string, object> userFieldData = new Dictionary<string, object>();

            DocumentReference docRef = collection.Document(userDocReferenceID);
            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                print("doc ref to trophies task");
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {


                    userFieldData = snapshot.ToDictionary();

                    foreach (KeyValuePair<string, object> pair in userFieldData)
                    {
                        Debug.Log(string.Format("{0}: {1}", pair.Key, pair.Value));
                        switch (pair.Key)
                        {
                            case "usertype":
                                if (pair.Value.ToString() == "Player")
                                    user.UserType = User.Type.User;
                                else if (pair.Value.ToString() == "Coordinator")
                                    user.UserType = User.Type.Coordinator;
                                break;

                            case "trophies":
                                print("Found Trophies!");
                                retrievedTrophyNames = pair.Value as List<object>;
                                TrophyManager.Instance.SessionTrophyTypesFlagsCount = retrievedTrophyNames.Count;
                                ResetUserTrophyTypes();
                                foreach (var trophyName in retrievedTrophyNames)
                                {
                                    trophyName.ToString();
                                    TrophyManager.Instance.AddATrophy((string)trophyName);
                                }

                                break;
                        }

                    }
                }

                else
                {
                    UIHandler.Instance.NotifyLoading = " ";
                    UIHandler.Instance.NotifyError = "User data doesn't exist. Contact a coordinator.";
                }
                TrophyManager.CurrentSessionTrophiesInitialized = true;
                TrophiesDataRetrieved = true;
            });

            #endregion
        }

        private IEnumerator Initializer()
        {
            while (!EventStateHandler.InitialStateRetrieved)
            {
                print("Retrieving intitial App state");
                yield return null;
            }

            if (EventStateHandler.CurrentState != EventStateHandler.AppState.Loading)
            {
                StartCoroutine(RetrieveUserDoccumentReferenceID());
                StartCoroutine(RetrieveUserData());
            }
            else { print("Can't initialize as state is AppState = Loading"); }
        }
        /// <summary>
        /// Logs the user in based on the login email text field and password text field.
        /// </summary>
        public void Login()
        {
            StartCoroutine(LoggingIn(emailLoginField.text, passwordLoginField.text));
        }

        /// <summary>
        /// Register the user in based on the register email text field and password text field.
        /// </summary>
        public void Register()
        {
            StartCoroutine(Registering(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
        }

        /// <summary>
        /// Logs our current user out
        /// </summary>
        public void Logout()
        {
            ES3.DeleteFile();
            PlayerPrefs.DeleteAll();
            auth.SignOut();
            EventStateHandler.CurrentState = EventStateHandler.AppState.LoggingIn;
        }
        #endregion

        /// <summary>
        /// Resets all of our data
        /// </summary>
        private void Reset()
        {
            confirmLoginText.text = "You logged out. Please login again.";
            warningLoginText.text = "";
            warningRegisterText.text = "";
            emailLoginField.text = "";
            emailRegisterField.text = "";
            passwordLoginField.text = "";
            passwordRegisterField.text = "";
            passwordRegisterVerifyField.text = "";
            usernameRegisterField.text = "";
            UserDocumentReferenceID = "";
            user = null;
            IsLoggedIn = false;
            TrophiesDataRetrieved = false;
        }

        public void Save()
        {
            ES3.Save("LoggedIn", IsLoggedIn);
        }
        void ResetUserTrophyTypes()
        {
            if (CurrentUser != null)
            {
                CurrentUser.UserTrophyType &= ~User.TrophyType.LacrosseChampionsFirst;
                CurrentUser.UserTrophyType &= ~User.TrophyType.LacrosseChampionsSecond;
                CurrentUser.UserTrophyType &= ~User.TrophyType.SoccerFinalistsFirst;
                CurrentUser.UserTrophyType &= ~User.TrophyType.SoccerFinalistsSecond;
                CurrentUser.UserTrophyType &= ~User.TrophyType.None;
                print("Resetted user trophy types enum.");
            }

        }
        public void Load()
        {
            throw new System.NotImplementedException();
        }
    }
}
