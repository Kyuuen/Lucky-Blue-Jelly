using QFramework;
using QFramework.Example;
using UnityEngine;
using UnityEngine.UI;

public class CounterAppController : MonoBehaviour, IController
{
    private Button mBtnAdd;
    private Text mCountText;

    private CounterAppModel mModel;

    private void Start()
    {
        mModel = this.GetModel<CounterAppModel>();

        mBtnAdd = transform.Find("AddBtn").GetComponent<Button>();
        mCountText = transform.Find("Count").GetComponent<Text>();

        mBtnAdd.onClick.AddListener(() => {
            this.SendCommand<IncreaseCountCommand>(); 
        });

        mModel.Count.Register(UpdateView).UnRegisterWhenGameObjectDestroyed(gameObject);

        mModel.Count.Value++;

    }

    void UpdateView(int value)
    {
        Debug.Log("Count now is: " + value);
        mCountText.text = value.ToString();
    }

    public IArchitecture GetArchitecture()
    {
        return CounterApp.Interface;
    }

    private void OnDestroy()
    {
        mModel = null;
    }
}
