using UnityEngine;
using QFramework;
using System.Collections.Generic;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;

public interface IDropBubbleSystem : ISystem
{
    public void DropBubble(ref GameObject bubble, Rigidbody2D bubbleRb);
    public void DrawToSpawnBubble(ref GameObject dropBubble, GameObject spawnBubble, Transform dropSpot);
    public void PostSpawnNewBubble(GameObject dropBubble, GameObject spawnBubble, ref Rigidbody2D dropRb, ref Rigidbody2D spawnRb);
    public GameObject SwitchBubble(ref GameObject dropBubble, ref GameObject spawnBubble, ref Rigidbody2D dropRb, ref Rigidbody2D spawnRb);

}
public class DropBubbleSystem : AbstractSystem, IDropBubbleSystem
{
    private IGameSceneModel gameSceneModel;

    private bool gameStarted;

    private GameObject temp;
    protected override void OnInit()
    {
        gameSceneModel = this.GetModel<IGameSceneModel>();

        this.RegisterEvent<OnGameStartEvent>(e =>
        {
            gameStarted = true;
        });
        this.RegisterEvent<GameOverEvent>(e =>
        {
            gameStarted = false;
        });
        this.RegisterEvent<OnLevelWinEvent>(e =>
        {
            gameStarted = false;
        });
        this.RegisterEvent<RestartLevelEvent>(e =>
        {
            gameStarted = true;
        });
        this.RegisterEvent<PlayOnEvent>(e =>
        {
            gameStarted = true;
            gameSceneModel.Move.Value += 20;
        });
        this.RegisterEvent<GamePauseEvent>(e =>
        {
            gameStarted = !e.gameIsPaused;
        });
        this.RegisterEvent<BoosterActivateEvent>(e =>
        {
            gameStarted = false;
        });
        this.RegisterEvent<BoosterInactivateEvent>(e =>
        {
            UniTask.Create(async () =>
            {
                await UniTask.WaitForSeconds(3);
                gameStarted = true;
            });
        });
        this.RegisterEvent<BoosterPopupActivateEvent>(e =>
        {
            gameStarted = false;
        });
    }

    public void DropBubble(ref GameObject bubble, Rigidbody2D bubbleRb)
    {
        if (!gameStarted) return;
        bubbleRb.bodyType = RigidbodyType2D.Dynamic;
        gameSceneModel.IsDropping = true;
        bubble.GetComponent<BubbleController>().ChangeMask(false);
        bubble = null;
        gameSceneModel.Move.Value--;
        if (gameSceneModel.Move.Value <= 0)
        {
            UniTask.Create(async () =>
            {
                await UniTask.Delay(500);
                this.SendEvent<GameOverEvent>();

            });
        }
    }

    public void PostSpawnNewBubble(GameObject dropBubble, GameObject spawnBubble, ref Rigidbody2D dropRb, ref Rigidbody2D spawnRb)
    {
        if (!gameStarted) return;
        dropRb = dropBubble.GetComponent<Rigidbody2D>();

        spawnRb = spawnBubble.GetComponent<Rigidbody2D>();

        spawnRb.bodyType = RigidbodyType2D.Static;
        gameSceneModel.IsDropping = false;
    }

    public void DrawToSpawnBubble(ref GameObject dropBubble, GameObject spawnBubble, Transform dropSpot)
    {
        if (!gameStarted) 
        {
            return; 
        }
        spawnBubble.transform.position = dropSpot.position;
        dropBubble = spawnBubble;
    }

    public GameObject SwitchBubble(ref GameObject dropBubble, ref GameObject spawnBubble, ref Rigidbody2D dropRb, ref Rigidbody2D spawnRb)
    {
        if (!gameStarted || dropBubble == null) return null;
        temp = new GameObject();
        GameObject catchExp = temp;

        temp.transform.position = dropBubble.transform.position;
        dropBubble.transform.position = spawnBubble.transform.position;
        spawnBubble.transform.position = temp.transform.position;

        temp = dropBubble;
        dropBubble = spawnBubble;
        spawnBubble = temp;

        dropRb = dropBubble.GetComponent<Rigidbody2D>();
        spawnRb = spawnBubble.GetComponent<Rigidbody2D>();
        return catchExp;
    }
}

