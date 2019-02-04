using System;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;
using Assets;

namespace Localisation {
    public class Localise : MonoBehaviour {
        //private static string _lang;
        public static Language userLanguage;
        private static XmlDocument _doc = new XmlDocument();
        private readonly string[] AvailableLanguages = { "EN", "FR", "DE", "RU", "JP" };

        private void Awake() {
            //_lang = AvailableLanguages[new System.Random().Next(0, AvailableLanguages.Length)];
            /// _lang = "EN";
            //Debug.Log("Language set to " + _lang);
            SetLanguage(0);
        }

        public static void SetLanguage(int languageId) {
            userLanguage = (Language)languageId;
            Debug.Log("Language set to: " + userLanguage.ToString());

            try {
                if(Application.loadedLevel == 1)
                    UpgradeMenu.LocaliseLanguage();
            } catch (NullReferenceException nrex) {
                Debug.LogException(nrex);
            }

           
        }

        public enum Language {
            EN = 0,
            FR = 1,
            ES = 2,
            FI = 3,
            JP = 4,
        }

        public static void LoadDoc(string document) {
            try {
                _doc.Load(Path.Combine(Application.dataPath, "LanguageLocalisation/" + document));
            }
            catch (NullReferenceException nrex) {
                Debug.LogException(nrex);
            }
        }

        public static string GetLang() {
            return userLanguage.ToString();
        }

        public static void UnloadDoc() {
            _doc = null;
        }

        /* Input: "xml_test"
         * Sample XML: <string text="xml_test">Hello World!</string>
         * Return Value: "Hello World!*/
        public static string GetString(string attribute) {//returns the specified attributes text from the loaded xml file 
            foreach (XmlNode item in _doc.GetElementsByTagName(userLanguage.ToString())) {
                foreach (XmlNode child in item.ChildNodes.Cast<XmlNode>().Where(child => child.Attributes["text"].Value == attribute)) {
                    return child.InnerText;
                }
            }
            return null;
        }

    }
}