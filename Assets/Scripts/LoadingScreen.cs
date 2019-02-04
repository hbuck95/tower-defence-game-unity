using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreen : MonoBehaviour {
    [SerializeField]
    private GameObject _loadPanel;
    [SerializeField]
    private Text _loadProgress, _loadTip, _loadTitle;
    private string[] _loadingTips = new string[5];
    private AsyncOperation _load;
    private System.Random _rng = new System.Random();

	private void Start () {
        _loadingTips[0] = "Demolitions units do 20% more damage to buildings.";
        _loadingTips[1] = "Don't forget to upgrade your units to make them more durable!";
        _loadingTips[2] = "Need to deploy more units? Buy the 'Increased Production' upgrade now!";
        _loadingTips[3] = "Heavy weapons units may be slow, but they do the highest amount of damage.";
        _loadingTips[4] = "Earn credits and experience by defeating enemy units and buildings.";
        Application.backgroundLoadingPriority = ThreadPriority.High;
	}
	
    public void LoadLevel(int sceneId) {
        _loadPanel.SetActive(true);
        StartCoroutine(Load(sceneId));
    }

    /*private IEnumerator LoadingText() {
        string s = "";
        int count = 0;
        while (!_load.isDone) {
            if (count == 2) {
                count = 0;
                s = "";
            }                
            s += '.';
            loadTitle.text = string.Format("Loading{0}..", s);
            count++;
            Debug.Log(count);
            yield return new WaitForSeconds(1);
        }

    }*/


    private IEnumerator Load(int sceneId) {
        //loadTip.text = _loadingTips[_rng.Next(_loadingTips.Length - 1)];
        _loadTitle.text = "Loading...";
        _load = Application.LoadLevelAsync(sceneId);
        //_load.allowSceneActivation = true;
        //StartCoroutine(LoadingText());
        while (!_load.isDone) {
            _loadProgress.text = string.Format("{0}%", Mathf.CeilToInt(_load.progress * 100));
            yield return null;
        }
    }
}
