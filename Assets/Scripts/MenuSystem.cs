using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


namespace Assets {
    public class MenuSystem : MonoBehaviour {
        [SerializeField]
        private Button _startGame;
        private bool _emailOk, _usernameOk, _passwordOK;

        public GameObject createAccountControls, loginControls, credits;
        public InputField regUsernameIPF, regPswdIPF, regPswdConfirmIPF, regEmailIPF, loginUsernameIPF, loginPswdIPF;
        public Image regUsernameCheck, regPassCheck, regPassConfirmCheck, regEmailCheck;
        public Sprite tickSprite, crossSprite;
        public Toggle rememberMe;
        public Text regErrorMessage, regUsernameErrorMessage, regPasswordErrorMessage, regEmailErrorMessage, loginErrorMessage, loggedInUserField;
        public Text loginOutButton;
        public OptionsMenu optionsMenu;
        public LoadingScreen lS;
        public AudioClip click;
        public AudioSource audioSource;

        private Image _regUsernameIpfImage, _regPswdIpfImage, _regPswdCheckIpfImage, _regEmailIpfImage;

        public static bool loggedIn = false;
        public static string loggedInUser;

        private void Start() {
            _regUsernameIpfImage = regUsernameIPF.GetComponent<Image>();
            _regPswdCheckIpfImage = regPswdConfirmIPF.GetComponent<Image>();
            _regPswdIpfImage = regPswdIPF.GetComponent<Image>();
            _regEmailIpfImage = regEmailIPF.GetComponent<Image>();

            loginOutButton.text = loggedIn ? "Logout" : "Login";

            /*if (loggedIn)
            {
                loginOutButton.text = "Logout";
            }
            else
            {
                loginOutButton.text = "Login";
            }*/
        }

        public void MouseOverButton() {
            audioSource.PlayOneShot(click, 0.1f);
        }

        public void ShowCreateAccount()
        {
            loginControls.SetActive(false);
            optionsMenu.Close();
            createAccountControls.SetActive(true);
            regErrorMessage.text = "";
            regErrorMessage.color = Color.red;
            ResetRegInputFields();
        }

        public void ShowLogin()
        {
            createAccountControls.SetActive(false);
            optionsMenu.Close();
            loginControls.SetActive(true);
            ResetLoginInputs();
            loginUsernameIPF.text = PlayerPrefs.GetString("SavedUsername");
            loginPswdIPF.text = PlayerPrefs.GetString("SavedPassword");
        }

        public void ResetLoginInputs()
        {
            loginUsernameIPF.text = "";
            loginPswdIPF.text = "";
            loginErrorMessage.text = "";
        }

        public void LoginUser()
        {
            StartCoroutine(Login());
        }

        public void CloseMenu(int menuId)
        {
            switch (menuId)
            {
                case 0:
                    regErrorMessage.text = "";
                    regErrorMessage.color = Color.red;
                    ResetRegInputFields();
                    createAccountControls.SetActive(false);
                    break;
                case 1:
                    loginControls.SetActive(false);
                    break;
                case 2:
                    optionsMenu.Close();
                    break;

            }
        }

        public void opencredits() {

            optionsMenu.Close();
            CloseMenu(0);
            CloseMenu(1);
            credits.SetActive(!credits.activeSelf);
        }

        public void SelectMenuOption(int optionId)
        {
            switch (optionId)
            {
                case 0:
                    optionsMenu.Close();
                    ShowCreateAccount();
                    break;
                case 1:
                    optionsMenu.Close();
                    LoginLogout();
                    break;
                case 2:
                    lS.LoadLevel(1);
                    //LoadScene(1);
                    break;
                case 3:
                    CloseMenu(0);
                    CloseMenu(1);
                    optionsMenu.Open();
                    break;
                case 4:
                    lS.LoadLevel(3);
                    //LoadScene(3);
                    break;
                case 5:
                    StartCoroutine(Logout(true));
                    break;
                case 6:
                    StartCoroutine(RegisterAccount());
                    break;
                case 7:
                    StartCoroutine(Login());
                    break;
            }
        }

        private void LoginLogout()
        {
            if (loggedIn)
                StartCoroutine(Logout(false));
            else
                ShowLogin();
        }

        private string Strip(string s)
        {
            return s.Substring(1, s.Length - 2);
        }

       /* public void LoadScene(int sceneId)
        {
            StartCoroutine(Load(sceneId));
        }*/

        public bool NullOrEmpty(string s)
        {
            return s == null || s == "";
        }

        public void CheckRegUsername()
        {

            if (NullOrEmpty(regUsernameIPF.text))
            {
                regUsernameErrorMessage.color = Color.red;
                regUsernameErrorMessage.text = "A username is required.";
                SetVerifySprite(0, true);
                _usernameOk = false;
                return;
            }

            StartCoroutine(VerifyUsername());
        }

        public void CheckRegEmail()
        {

            if (NullOrEmpty(regEmailIPF.text))
            {
                regEmailErrorMessage.color = Color.red;
                Debug.Log("An email address is required!");
                regEmailErrorMessage.text = "An email address is required!";
                SetVerifySprite(3, true);
                _emailOk = false;
                return;
            }

            if (!regEmailIPF.text.Contains("@"))
            {
                Debug.Log("Email address not valid.");
                regErrorMessage.color = Color.red;
                regEmailErrorMessage.text = "Email address not valid.";
                SetVerifySprite(3, true);
                _emailOk = false;
                return;
            }

            StartCoroutine(VerifyEmail());
        }

        private void ResetRegInputFields()
        {//Reset the input fields to their default states (colour, text etc.)
            regUsernameIPF.text = "";
            regEmailIPF.text = "";
            regPswdIPF.text = "";
            regPswdConfirmIPF.text = "";
            regEmailErrorMessage.text = "";
            regUsernameErrorMessage.text = "";
            regPasswordErrorMessage.text = "";
            regEmailErrorMessage.color = Color.red;
            regPasswordErrorMessage.color = Color.red;
            regUsernameErrorMessage.color = Color.red;
            _regUsernameIpfImage.color = Color.white;
            _regEmailIpfImage.color = Color.white;
            _regPswdCheckIpfImage.color = Color.white;
            _regPswdIpfImage.color = Color.white;
            regUsernameCheck.enabled = false;
            regPassCheck.enabled = false;
            regPassConfirmCheck.enabled = false;
            regEmailCheck.enabled = false;
        }

        public void CheckPassword()
        {
            string password = regPswdIPF.text;
            string confirmPassword = regPswdConfirmIPF.text;
            _passwordOK = false;

            if (NullOrEmpty(password))
            {
                Debug.Log("A password is required!");
                regPasswordErrorMessage.text = "A password is required!";
                SetVerifySprite(1, true);
                return;
            }

            if (password.Contains(" ") || password.Contains("%20"))
            {
                Debug.Log("Passwords cannot contain spaces!");
                regPasswordErrorMessage.text = "Passwords cannot contain spaces.";
                SetVerifySprite(1, true);
                return;
            }

            if (password.Length < 8)
            {
                Debug.Log("Passwords must be between 8 and 25 characters long.");
                regPasswordErrorMessage.text = "Passwords must be between 8 and 25 characters long.";
                SetVerifySprite(1, true);
                return;
            }

            if (!password.Any(char.IsUpper) || !password.Any(char.IsNumber))
            {
                Debug.Log("Passwords must contain at least 1 uppercase and 1 numeric character!");
                regPasswordErrorMessage.text = "Passwords must contain at least 1 uppercase and 1 numeric character!";
                SetVerifySprite(1, true);
                return;
            }

            if (confirmPassword != password)
            {
                Debug.Log("Entered passwords dont match!");
                regPasswordErrorMessage.text = "Entered password don't match!";
                SetVerifySprite(1, true);
                SetVerifySprite(2, true);
                return;
            }
            else
            {
                SetVerifySprite(2, false);
            }

            SetVerifySprite(1, false);
            regPasswordErrorMessage.text = "";
            _passwordOK = true;
        }

   /*     private IEnumerator Load(int sceneId)
        {
            _loadPanel.SetActive(true);
            AsyncOperation load = Application.LoadLevelAsync(sceneId);
            while (!load.isDone)
            {
                _loadProgressBar.text = string.Format("{0}%...", Mathf.CeilToInt(load.progress * 100));
                Debug.Log(load.progress);
                yield return null;
            }
        }*/

        private IEnumerator<WWW> RegisterAccount()
        {
            regErrorMessage.text = "";
            CheckRegUsername();
            CheckRegEmail();
            CheckPassword();

            if (!_usernameOk || !_emailOk || !_passwordOK)
            {
                Debug.Log("Issues with username, email, or password.");
                yield break;
            }

            var request = new WWW(string.Format("http://localhost:3034/api/player/register?username={0}&password={1}&emailAddress={2}", regUsernameIPF.text.Replace(" ", "%20"), regPswdIPF.text, regEmailIPF.text));
            yield return request;
            Debug.Log(request.text);

            regErrorMessage.color = Color.red;

            if (NullOrEmpty(request.text))
            {
                Debug.Log("Error connecting to server.");
                regErrorMessage.text = "Error connecting to server.";
                yield break;
            }

            switch (int.Parse(request.text))
            {
                case -1:
                    regErrorMessage.text = "An error occured whilst processing your request.";
                    break;
                case 1:
                    regErrorMessage.color = Color.blue;
                    regErrorMessage.text = "Account created.";
                    ResetRegInputFields();
                    break;
                case 0:
                    regUsernameErrorMessage.text = "Username already in use.";
                    break;
                case 2:
                    regEmailErrorMessage.text = "Email address already in use.";
                    break;
                default:
                    regErrorMessage.text = string.Format("Unexpected Error!\n{0}", request.text);
                    break;
            }
        }

        private IEnumerator<WWW> Logout(bool quit)
        {
            var request = new WWW(string.Format("http://localhost:3034/api/player/logout?username={0}", loggedInUser));
            yield return request;

            if (NullOrEmpty(request.text))
            {
                Debug.Log("Error connecting to server.");
                yield break;
            }

            loggedInUserField.text = "";
            loggedInUser = null;
            loggedIn = false;
            loginOutButton.text = "Login";

            if (quit)
                Application.Quit();
        }

        private IEnumerator<WWW> Login()
        {

            //disable the input fields to make sure nothing can be editted during the login process.
            loginUsernameIPF.interactable = false;
            loginPswdIPF.interactable = false;

            /*if(!loginPswdIPF.text.Any(char.IsUpper) || !loginPswdIPF.text.Any(char.IsNumber) || loginPswdIPF.text.Length < 8) {
                loginErrorMessage.text = "Invalid username or password.";
                yield break;
            }*/

            var request = new WWW(string.Format("http://localhost:3034/api/player/login?username={0}&password={1}", loginUsernameIPF.text, loginPswdIPF.text));
            yield return request;

            Debug.Log(string.Format("Login Request: {0}", request.text));

            if (NullOrEmpty(request.text))
            {
                loginErrorMessage.text = "Error connecting to server.";
                Debug.Log("Error connecting to server.");
                loginUsernameIPF.interactable = true;
                loginPswdIPF.interactable = true;
                yield break;
            }

            if (rememberMe.isOn)
            {
                PlayerPrefs.SetString("RememberMe", "1");
                PlayerPrefs.SetString("SavedUsername", loginUsernameIPF.text);
                PlayerPrefs.SetString("SavedPassword", loginPswdIPF.text);
            }
            else
            {
                PlayerPrefs.SetString("RememberMe", "0");
            }

            switch (int.Parse(request.text))
            {
                case -1:
                    Debug.Log("An error occured whilst processing your request.");
                    loginErrorMessage.text = "An error occured whilst processing your request.";
                    break;
                case 0:
                    Debug.Log("Username or password don't match!");
                    loginErrorMessage.text = "Invalid username or password.";
                    break;
                case 1:
                    Debug.Log("Your account is already logged in!");
                    loginErrorMessage.text = "Your account is already logged in!";
                    break;
                case 2:
                    Debug.Log("Login successful!");
                    CloseMenu(1);
                    StartCoroutine(GetUsername());
                    break;
                default:
                    Debug.Log(string.Format("Unexpected Error!\n{0}", request.text));
                    break;
            }

            //renable them after the process has finished
            loginUsernameIPF.interactable = true;
            loginPswdIPF.interactable = true;
        }

        private IEnumerator<WWW> GetUsername()
        {
            var request = new WWW(string.Format("http://localhost:3034/api/player/getusername?username={0}", loginUsernameIPF.text));
            yield return request;
            Debug.Log(string.Format("GetUsername Request: {0}", request.text));
            if (!NullOrEmpty(request.text))
            {
                loggedInUser = Strip(request.text);
                loggedIn = true;
                loggedInUserField.text = string.Format("Welcome {0}!", loggedInUser);
                loginOutButton.text = "Logout";
            }
            else
            {
                loginErrorMessage.text = "Error communicating with server!";
            }
        }

        private IEnumerator VerifyUsername()
        {
            var username = regUsernameIPF.text.Replace(" ", "%20");

            var request = new WWW(string.Format("http://localhost:3034/api/player/register/checkusername?username={0}", username));
            yield return request;

            Debug.Log(request.text);

            if (request.text == "" || request.text == null)
            {
                regErrorMessage.text = "Error connecting to server.";
                yield break;
            }

            bool response = bool.Parse(request.text);

            if (response)
            {
                regUsernameErrorMessage.color = Color.red;
                regUsernameErrorMessage.text = "Username already taken!";
            }
            else
            {
                regUsernameErrorMessage.color = Color.blue;
                regUsernameErrorMessage.text = "Username available!";
            }

            _usernameOk = !response;
            SetVerifySprite(0, response);
        }

        private IEnumerator VerifyEmail()
        {
            var request = new WWW(string.Format("http://localhost:3034/api/player/register/checkemail?email={0}", regEmailIPF.text));
            yield return request;

            if (request.text == "" || request.text == null)
            {
                regErrorMessage.text = "Error connecting to server.";
                _emailOk = false;
                yield break;
            }

            bool response = bool.Parse(request.text);

            if (response)
            {
                regEmailErrorMessage.text = "Email address already in use!";
            }
            else
            {
                regEmailErrorMessage.text = "";
            }

            _emailOk = !response;
            SetVerifySprite(3, response);
        }

        private void SetVerifySprite(int component, bool status)
        {
            Image verificationImage = null;
            Image ipfImage = null;

            switch (component)
            {
                case 0:
                    verificationImage = regUsernameCheck;
                    ipfImage = _regUsernameIpfImage;
                    break;
                case 1:
                    verificationImage = regPassCheck;
                    ipfImage = _regPswdIpfImage;
                    break;
                case 2:
                    verificationImage = regPassConfirmCheck;
                    ipfImage = _regPswdCheckIpfImage;
                    break;
                case 3:
                    verificationImage = regEmailCheck;
                    ipfImage = _regEmailIpfImage;
                    break;
            }

            verificationImage.enabled = true;

            if (status)
            {
                verificationImage.sprite = crossSprite;
                verificationImage.color = Color.red;
                ipfImage.color = Color.white;
                if (component == 2)
                    _regPswdCheckIpfImage.color = Color.white;
            }
            else
            {
                verificationImage.sprite = tickSprite;
                verificationImage.color = Color.green;
                ipfImage.color = Color.green;
                if (component == 2)
                    _regPswdCheckIpfImage.color = Color.green;
            }
        }

    }
}