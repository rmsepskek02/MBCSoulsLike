using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BS.Title
{
    
    public class StageMove : MonoBehaviour
    {
        public Button button1;
        public Button button2;
        public Button button3;
        public Button button4;
        public string sceneName1;
        public string sceneName2;
        public string sceneName3;
        public string sceneName4;
        void Start()
        {
            button1.onClick.AddListener(() => LoadScene(sceneName1));
            button2.onClick.AddListener(() => LoadScene(sceneName2));
            button3.onClick.AddListener(() => LoadScene(sceneName3));
            button4.onClick.AddListener(() => LoadScene(sceneName4));
        }

       public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }

}