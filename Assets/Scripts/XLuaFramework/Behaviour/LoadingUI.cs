using UnityEngine;
using UnityEngine.UI;

namespace XLuaFramework.Behaviour
{
    public class LoadingUI : MonoBehaviour
    {
    
        [SerializeField] private Text progressDesc;

        [SerializeField] private Slider progressBar;
    
        private float _max;

        public void InitProgress(float max, string desc)
        {
            _max = max;
            progressBar.gameObject.SetActive(true);
            progressDesc.gameObject.SetActive(true);

            progressDesc.text = desc;
            progressBar.value = max > 0 ? 0 : 100;
        
        }

        public void UpdateProgress(float progress)
        {
            progressBar.value = progress / _max;
        }
    }
}
