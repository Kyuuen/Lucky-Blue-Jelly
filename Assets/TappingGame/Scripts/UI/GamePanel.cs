using QFramework;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour, IController
{
    [SerializeField] private Text mTimeText;
    [SerializeField] private Button mStartButton;
    [SerializeField] private Button mRestartButton;

    private CountDownSystem mCountDownSystem;

    private void Awake()
    {
        mCountDownSystem = this.GetSystem<CountDownSystem>();

        mStartButton.onClick.AddListener(() =>
        {
            mStartButton.gameObject.SetActive(false);
            mRestartButton.gameObject.SetActive(false);
            mTimeText.gameObject.SetActive(true);
            this.SendCommand<StartGameCommand>();
        });

        mRestartButton.onClick.AddListener(() =>
        {
            mRestartButton.gameObject.SetActive(false);
            mTimeText.gameObject.SetActive(true);
            this.SendCommand<StartGameCommand>();
        });

        this.RegisterEvent<OnCountDownEvent>(e =>
        {
            Debug.Log($"Log data {e.isGameEnd}");
            mRestartButton.gameObject.SetActive(true);
            mTimeText.gameObject.SetActive(false);
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        this.RegisterEvent<GamePassEvent>(e =>
        {
            mRestartButton.gameObject.SetActive(true);
            mTimeText.gameObject.SetActive(false);
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

    }

    private void OnCountDownEnd(OnCountDownEvent e)
    {
        Debug.Log($"Log data {e.isGameEnd}");
        mRestartButton.gameObject.SetActive(true);
        mTimeText.gameObject.SetActive(false);
    }
   
    private void Update()
    {
        if (Time.frameCount % 20 == 0)
        {
            mTimeText.text = mCountDownSystem.Time + "s";

            mCountDownSystem.Tick();
        }
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return TappingGame.Interface;
    }
}
