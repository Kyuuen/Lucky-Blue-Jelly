using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    public class BindablePropertyExample : MonoBehaviour
    {
        private BindableProperty<int> mSomeValue = new BindableProperty<int>(0);

        private BindableProperty<string> mName = new BindableProperty<string>("QFramework");

        private Text mValueText;
        
        void Start()
        {
            mValueText = transform.Find("Text").GetComponent<Text>();
            mSomeValue.Register(newValue =>
            {
                Debug.Log(newValue);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            mName.RegisterWithInitValue(newName =>
            {
                Debug.Log(mName);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
             
            mSomeValue.RegisterWithInitValue(newcount =>
            {
                UpdateView();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        
        void UpdateView()
        {
            mValueText.text = mSomeValue.ToString();
        }
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                mSomeValue.Value++;
            }
        }
    }
}
