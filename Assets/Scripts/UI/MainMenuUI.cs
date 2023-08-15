using UnityEditor;
using UnityEngine;

namespace UI
{
    public class MainMenuUI : UI
    {
        public void Quit()
        {
            #if (UNITY_EDITOR)
            EditorApplication.ExitPlaymode();
            #elif (UNITY_STANDALONE) 
            Application.Quit();
            #elif (UNITY_WEBGL)
            Application.ExternalEval("window.open('" + "about:blank" + "','_self')");
            #endif
        }
    }
}
