using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MainTheme : MonoBehaviour {

    public static MainTheme theme;
    public string sceneName;
    

    // Use this for initialization

    void Awake () {
         if (theme == null)
         {
            DontDestroyOnLoad(gameObject);
            theme = this;
         }
         else if (theme != this)
         {
             Destroy(gameObject);
         }
    }
	
	// Update is called once per frame
	void Update () {
        Scene scene = SceneManager.GetActiveScene();
        //Debug.Log(scene.name);

        if (scene.name == "Level1" || scene.name == "Level2")
        {
            //audio.volume = 0.2F;
            GetComponent<AudioSource>().volume = 0.0f;
        } else
        {
            GetComponent<AudioSource>().volume = 1.0f;
        }

    }
}
