using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;
using UnityEngine.UI;
//(GameManager)https://dkrevel.com/makegame-beginner/make-2d-action-game-manager/

public class GM : MonoBehaviour
{
    public static GM instance = null;
    [SerializeField]
    public int gmCount;
    //public Image image;
    [SerializeField]
    public string nameGM;
    public int sceneCount;
    public float predictionTime60;//—\‘zŠÔ
    public float stopTime60;
    public float runTime60;
    public float travelTime60;

    public float predictionTime40;
    public float stopTime40;
    public float runTime40;
    public float travelTime40;

    //chat1 chat1;

    private void Awake()//GM2
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*public void SceneCount()
    {
        gmCount++;
        if (gmCount >= 3)
        {
            Debug.Log("gmCount‚ª3ˆÈã‚¾");
            //image.color = Color.red;
            SceneManager.LoadScene("input1Screen");
        }
    }*/
    
}
