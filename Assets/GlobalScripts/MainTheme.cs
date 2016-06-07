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
        
        if (scene.name.Substring(0,5) == "Level")
        {
            //audio.volume = 0.2F;
            GetComponent<AudioSource>().volume = 0.0f;
            GetComponent<AudioSource>().time = 0;
        } else
        {
            GetComponent<AudioSource>().volume = 0.5f;
        }

    }
}
