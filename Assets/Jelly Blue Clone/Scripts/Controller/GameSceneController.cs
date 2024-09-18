
using UnityEngine;
using QFramework;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Collections;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Unity.VisualScripting;
using DG.Tweening;
using UnityEngine.AddressableAssets;
using System.Runtime.CompilerServices;
using UnityEngine.EventSystems;
using ZBase.UnityScreenNavigator.Core.Modals;
using UnityEngine.UI;

public class GameSceneController : MonoBehaviour, IController
{
    [SerializeField] private LevelObjectSpawner _thisLevel;
    [SerializeField] private List<GameObject> _bubbles;
    [SerializeField] private Transform _dropSpot;
    [SerializeField] private Transform _spawnSpot;

    [SerializeField] private GameObject _cloudUI;
    [SerializeField] private GameObject _confettiUI;
    [SerializeField] private GameObject _iceObject;
    private float jellyMoveDuration = 0.15f;

    private GameObject _coinUI;
    private GameObject _spawnBubble;
    private GameObject _dropBubble;
    private GameObject _bubbleToSpawn;
    private GameObject _jellyToSpawn;

    private BubbleController _breakBubble;
    private BubbleController _mergeBubble;

    private Rigidbody2D _dropBubbleRb;
    private Rigidbody2D _spawnBubbleRb;

    private float _dropDelay;
    private int _boosterType;
    private bool _boosterIsOn;
    [SerializeField] private LayerMask clickableLayer;
    [SerializeField] private LayerMask mapLayer;

    //Limit of dragging object
    private Vector3 offset;
    private Vector3 extents;
    [SerializeField] Transform leftBorder;
    [SerializeField] Transform rightBorder;

    [SerializeField] private List<GameObject> _bubbleSizePrefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> _jellyPrefabs = new List<GameObject>();

    private BubbleInfo _bubbleInfo;
    private int _bubbleIds;
    private int _iceIds;
    private float _radiusReduceAmount = 0.015f;

    private Vector3 goldPos;

    private bool _gameIsEnd;
    private bool _isDragging;
    bool vibrationIsAvailable = true;

    private IMouseMovementSystem _mouseMovementSystem;
    private IDropBubbleSystem _dropBubbleSystem;
    private ISpawnJellySystem _spawnJellySystem;
    private ILevelStartSpawnSystem _levelStartSpawnSystem;
    private IUISystem _uiSystem;

    private IGameSceneModel _gameSceneModel;
    private IPlayerPrefModel _playerPrefModel;

    private void Awake()
    {
        _mouseMovementSystem = this.GetSystem<IMouseMovementSystem>();
        _dropBubbleSystem = this.GetSystem<IDropBubbleSystem>();
        _spawnJellySystem = this.GetSystem<ISpawnJellySystem>();
        _levelStartSpawnSystem = this.GetSystem<ILevelStartSpawnSystem>();
        _uiSystem = this.GetSystem<IUISystem>();

        _gameSceneModel = this.GetModel<IGameSceneModel>();
        _playerPrefModel = this.GetModel<IPlayerPrefModel>();

        _bubbles = new List<GameObject>();
        _gameIsEnd = false;
        _boosterIsOn = false;
        _isDragging = false;
        _dropDelay = 0;
        vibrationIsAvailable = _playerPrefModel.VibrateAvailable.Value;

        this.RegisterEvent<OnGameStartEvent>(OnGameStart).UnRegisterWhenGameObjectDestroyed(gameObject.transform.parent.gameObject);
        this.RegisterEvent<DroppedBubbleEvent>(DroppedBubble).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<SwitchBubbleEvent>(SwitchBubble).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<BoosterActivateEvent>(BoosterActivate).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<MergeBubbleEvent>(MergeBubble).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<PlayOnEvent>(PlayOn).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<DoneBubbleEvent>(DoneBubble).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnLevelWinEvent>(OnLevelWin).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<GameOverEvent>(GameOver).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<PlayHammerAniEvent>(PlayHammerAni).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<PlayStrawberryAniEvent>(PlayStrawberryAni).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<PlayFirecrackerAniEvent>(PlayFirecrackerAni).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<StrawberryBreakEvent>(StrawberryBreak).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<BoosterInactivateEvent>(e =>
        {
            _boosterIsOn = false;
        });

        this.RegisterEvent<VibrationEnableEvent>(e =>
        {
            vibrationIsAvailable = e.vibrationIsEnable;
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    void OnGameStart(OnGameStartEvent e)
    {
        _bubbleIds = 0;
        _iceIds = 0;
        _spawnJellySystem.GetColorSpawnRate(_thisLevel.ColorSpawnRate);
        foreach (var bubble in _thisLevel.bubbles)
        {
            _levelStartSpawnSystem.SetBubbleToSpawn(bubble);
            _bubbleInfo = _gameSceneModel.Bubbles[_bubbleIds];
            _bubbles.Add(GetStartBubbleToSpawn(bubble.pos));
            _bubbleIds++;
        }

        this.SendCommand<OnNewBubbleSpawnCommand>();
        _bubbleInfo = _gameSceneModel.Bubbles[_bubbleIds];
        _dropBubble = GetBubbleToSpawn(_dropSpot.position);
        _dropBubble.GetComponent<BubbleController>().ChangeMask(true);
        _bubbles.Add(_dropBubble);
        _bubbleIds++;

        SpawnNewBubbleAtSpawn();

        _dropBubbleRb = _dropBubble.GetComponent<Rigidbody2D>();
        _spawnBubbleRb = _spawnBubble.GetComponent<Rigidbody2D>();

        _spawnBubbleRb.bodyType = RigidbodyType2D.Static;
        _dropBubbleRb.bodyType = RigidbodyType2D.Static;

        if (_thisLevel.icePos != null) SpawnIce();
    }

    void PlayOn(PlayOnEvent e)
    {
        _gameIsEnd = false;
        UniTask.Create(async () =>
        {
            await UniTask.WaitForSeconds(0.3f);
            int deletedCount = 0;
            for (int i = _bubbles.Count - 3; i >= 0; i--)
            {
                if (deletedCount == 5) break;
                if (_bubbles[i] != null)
                {
                    _bubbles[i].GetComponent<BubbleController>().IsMerge();
                    deletedCount++;
                }
            }
        });
        //_dropBubbleSystem.DrawToSpawnBubble(ref _dropBubble, _spawnBubble, _dropSpot);
        //SpawnNewBubbleAtSpawn();
        //_dropBubbleSystem.PostSpawnNewBubble(_dropBubble, _spawnBubble, ref _dropBubbleRb, ref _spawnBubbleRb);
    }

    void DroppedBubble(DroppedBubbleEvent e)
    {
        if (_gameIsEnd) return;
        _dropBubbleSystem.DrawToSpawnBubble(ref _dropBubble, _spawnBubble, _dropSpot);
        SpawnNewBubbleAtSpawn();
        _dropBubbleSystem.PostSpawnNewBubble(_dropBubble, _spawnBubble, ref _dropBubbleRb, ref _spawnBubbleRb);
        _dropDelay = 0.5f;
    }

    void SpawnNewBubbleAtSpawn()
    {
        this.SendCommand<OnNewBubbleSpawnCommand>();
        _bubbleInfo = _gameSceneModel.Bubbles[_bubbleIds];
        _spawnBubble = GetBubbleToSpawn(_spawnSpot.position);
        _spawnBubble.GetComponent<BubbleController>().ChangeMask(true);

        _bubbles.Add(_spawnBubble);
        _bubbleIds++;
    }

    void SwitchBubble(SwitchBubbleEvent e)
    {
        Destroy(_dropBubbleSystem.SwitchBubble(ref _dropBubble, ref _spawnBubble, ref _dropBubbleRb, ref _spawnBubbleRb));
    }

    void BoosterActivate(BoosterActivateEvent e)
    {
        _boosterIsOn = true;
        switch (e._boosterStatus)
        {
            case 0:
                {
                    _boosterType = e._boosterStatus;
                    foreach (int i in e._bubbleIDs)
                    {
                        if (_bubbles[i] != null)
                        {
                            _bubbles[i].GetComponent<BubbleController>().BoosterActivate(e._boosterStatus);
                        }
                    }
                    break;
                }
            case 1:
                {
                    _boosterType = e._boosterStatus;
                    foreach (int i in e._bubbleIDs)
                    {
                        if (_bubbles[i] != null)
                        {
                            _bubbles[i].GetComponent<BubbleController>().BoosterActivate(e._boosterStatus);
                        }
                    }
                    break;
                }
            case 2:
                {
                    _boosterType = e._boosterStatus;
                    break;
                }
        }
    }

    async void PlayHammerAni(PlayHammerAniEvent e)
    {
        GameObject newAni = await GetBooster(0);
        Instantiate(newAni).GetComponent<HammerAnimation>().PlayAnimation(e._bubble, e._iD);
    }

    async void PlayStrawberryAni(PlayStrawberryAniEvent e)
    {
        GameObject newAni = await GetBooster(1);
        Instantiate(newAni).GetComponent<StrawberryAnimation>().PlayAnimation();
    }

    async void PlayFirecrackerAni(PlayFirecrackerAniEvent e)
    {
        GameObject newAni = await GetBooster(2);
        Instantiate(newAni).GetComponent<FireCrackerAnimation>().PlayAnimation(e._mousePos);
    }

    async void StrawberryBreak(StrawberryBreakEvent e)
    {
        await UniTask.Delay(2000);
        foreach (int id in e._iDs)
        {
            if (_bubbles[id] != null)
                _bubbles[id].GetComponent<BubbleController>().IsDone();
        }
    }

    async UniTask<GameObject> GetBooster(int boosterType)
    {
        UniTask<GameObject> asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>(ResourceKeys.BoosterAniPrefab(boosterType)).Task.AsUniTask();
        GameObject result = await asyncOperationHandle;
        return result;
    }

    void OnLevelWin(OnLevelWinEvent e)
    {
        _gameIsEnd = true;
        //Destroy(_bubbles[_bubbleIds]);
        //Destroy(_bubbles[_bubbleIds - 1]);
        goldPos = _uiSystem.ConVertUIToWorld(_uiSystem.GoldUIPos);
        UniTask.Create(async () =>
        {
            await UniTask.Delay(2000);
            GameObject newConfetti = Instantiate(_confettiUI, new Vector3(0, -1.5f, 100), Quaternion.identity);
            Destroy(newConfetti, 5);
        });
        Invoke(nameof(CalculateScore), 5f);
    }
    async void CalculateScore()
    {
        List<int> idBubblesLeft = new List<int>();
        for (int i = 0; i < _bubbleIds - 2; i++)
        {
            if (_bubbles[i] != null)
            {
                idBubblesLeft.Add(_bubbles[i].GetComponent<BubbleController>().iD);
                await UniTask.Delay(250);
                SpawnRayToGold(_bubbles[i]);
            }
        }

        this.SendCommand(new OnLevelWinCommand()
        {
            idBubblesLeft = idBubblesLeft
        });
    }

    async void SpawnRayToGold(GameObject bubble)
    {
        GameObject newObject = Instantiate(_cloudUI, goldPos, Quaternion.identity);
        Rigidbody2D rb = newObject.AddComponent<Rigidbody2D>();
        bubble.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        UniTask<GameObject> asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>(ResourceKeys.BunchOfCoinPrefab()).Task.AsUniTask();
        _coinUI = await asyncOperationHandle;
        rb.DOMove(bubble.transform.position, 0.5f).OnComplete(() =>
        {
            CoinMovement(Instantiate(_coinUI, bubble.transform.position, Quaternion.identity), goldPos, 0.75f);
            Destroy(bubble);
            Destroy(newObject);
        });

    }

    void CoinMovement(GameObject coin, Vector2 destination, float duration)
    {
        coin.GetComponent<BunchOfCoin>().PlayCoinCollectAnimation(destination, duration);
    }

    void GameOver(GameOverEvent e)
    {
        //if(_dropBubble != null) Destroy(_dropBubble);
    }

    GameObject GetBubbleToSpawn(Vector2 spot)
    {
        _bubbleToSpawn = _bubbleSizePrefabs[_bubbleInfo.Size];
        GameObject newBubble = Instantiate(_bubbleToSpawn, spot, Quaternion.identity);
        BubbleController _bubbleController = newBubble.GetComponent<BubbleController>();
        _bubbleController.iD = _bubbleIds;
        _bubbleController.maxNumb = _bubbleInfo.MaxNumb;
        Vector3 spawnOffset = new Vector3(-0.05f, -0.05f, 0);

        foreach (int type in _bubbleInfo.jellyColor)
        {
            _jellyToSpawn = _spawnJellySystem.GetJellyToSpawn(type, _jellyPrefabs);

            GameObject newJelly = Instantiate(_jellyToSpawn, newBubble.transform.position + spawnOffset, Quaternion.identity);
            spawnOffset += new Vector3(0.01f, 0.01f, 0);
            newJelly.transform.SetParent(newBubble.transform, true);
            _bubbleController.jellies.Add(newJelly);
        }
        newBubble.GetComponent<BubbleController>().ConnectToBubble();
        return newBubble;
    }

    GameObject GetStartBubbleToSpawn(Vector2 spot)
    {
        _bubbleToSpawn = _bubbleSizePrefabs[_bubbleInfo.Size];
        GameObject newBubble = Instantiate(_bubbleToSpawn, spot, Quaternion.identity);
        BubbleController newBubbleController = newBubble.GetComponent<BubbleController>();
        newBubbleController.iD = _bubbleIds;
        newBubbleController.IsSpawnEarly();
        newBubbleController.maxNumb = _bubbleInfo.MaxNumb;
        Rigidbody2D bubbleRigidbody = newBubble.GetComponent<Rigidbody2D>();
        bubbleRigidbody.bodyType = RigidbodyType2D.Static;

        // Start the coroutine to spawn jellies
        SpawnJellies(newBubble, spot, bubbleRigidbody);
        return newBubble;
    }

    private void SpawnJellies(GameObject newBubble, Vector2 spot, Rigidbody2D bubbleRigidbody)
    {
        BubbleController _bubbleController = newBubble.GetComponent<BubbleController>();
        Vector2 spawnOffset = new Vector2(0.01f, 0.01f);
        foreach (int type in _bubbleInfo.jellyColor)
        {
            GameObject _jellyToSpawn = _spawnJellySystem.GetJellyToSpawn(type, _jellyPrefabs);
            GameObject newJelly = Instantiate(_jellyToSpawn, spot + spawnOffset, Quaternion.identity);
            spawnOffset += new Vector2(0.01f, 0.01f);
            newJelly.transform.SetParent(newBubble.transform, true);
            _bubbleController.jellies.Add(newJelly);
        }
        newBubble.GetComponent<BubbleController>().ConnectToBubble();
        bubbleRigidbody.bodyType = RigidbodyType2D.Dynamic;
        if (_bubbleIds == _thisLevel.bubbles.Length - 1)
        {
            _spawnJellySystem.gameStarting = false;
        }
    }

    public void SpawnIce()
    {
        foreach (var destination in _thisLevel.icePos)
        {
            GameObject newIce = Instantiate(_iceObject, destination, Quaternion.identity);
            newIce.transform.SetParent(this.transform);
            newIce.GetComponent<IceController>().id = _iceIds;
            _iceIds++;
        }
    }

    void MergeBubble(MergeBubbleEvent e)
    {
        AudioManager.Instance.PlayBubbleSound(2);
        if (e.mergeStatus == 1)
        {
            MergeTwoSingleColorBubble(e.mergeId, e.breakId);
        }
        else if (e.mergeStatus == 2)
        {
            MergeOneSingleToOneMulty(e.mergeId, e.breakId, e.colorType);
        }
        else if (e.mergeStatus == 3)
        {
            TradeBubble(e.mergeId, e.breakId, e.colorType, e.moveBack);
        }
        else if (e.mergeStatus == 4)
        {
            TradeBubbleWithTwoSameColor(e.mergeId, e.breakId, e.colorTypes, e.moveBack);
        }
    }

    void MergeTwoSingleColorBubble(int mergeId, int breakId)
    {
        _mergeBubble = _bubbles[mergeId].GetComponent<BubbleController>();
        _breakBubble = _bubbles[breakId].GetComponent<BubbleController>();

        _breakBubble.DisConnectToType(_breakBubble.jellies[0].GetComponent<JellyController>().Type, true);
        foreach (var jelly in _breakBubble.jellies)
        {
            jelly.GetComponent<JellyController>().PlayMoveAnimate(_mergeBubble.gameObject, jellyMoveDuration);
            _mergeBubble.jellies.Add(jelly);
        }

        foreach (var jelly in _mergeBubble.jellies)
        {
            jelly.GetComponent<JellyController>().ReduceColliderRadius(_radiusReduceAmount * _mergeBubble.jellies.Count);
        }

        _breakBubble.jellies.Clear();
        _breakBubble.IsMerge();
    }

    void MergeOneSingleToOneMulty(int mergeId, int breakId, int colorType)
    {
        _mergeBubble = _bubbles[mergeId].GetComponent<BubbleController>();
        _breakBubble = _bubbles[breakId].GetComponent<BubbleController>();
        List<GameObject> jellies = new List<GameObject>();
        _breakBubble.DisConnectToType(colorType, true);
        foreach (var jelly in _breakBubble.jellies)
        {
            if (jelly.GetComponent<JellyController>().Type == colorType)
            {
                jellies.Add(jelly);
            }
        }

        foreach (var jelly in jellies)
        {
            jelly.GetComponent<JellyController>().PlayMoveAnimate(_mergeBubble.gameObject, jellyMoveDuration);
            _mergeBubble.jellies.Add(jelly);
            _breakBubble.jellies.Remove(jelly);
        }

        foreach (var jelly in _mergeBubble.jellies)
        {
            jelly.GetComponent<JellyController>().ReduceColliderRadius(_radiusReduceAmount * _mergeBubble.jellies.Count);
        }

    }

    void TradeBubble(int mergeId, int breakId, int colorType, bool moveBack)
    {
        // Lấy thông tin các bubble từ mergeId và breakId
        _mergeBubble = _bubbles[mergeId].GetComponent<BubbleController>();
        _breakBubble = _bubbles[breakId].GetComponent<BubbleController>();
        List<GameObject> temp = new List<GameObject>();

        // Ngắt kết nối các jelly màu tương ứng từ breakBubble
        _breakBubble.DisConnectToType(colorType, true);
        _mergeBubble.DisConnectToType(colorType, false);

        // Tìm các jelly có màu tương ứng (colorType) từ breakBubble để di chuyển sang mergeBubble
        foreach (var jelly in _breakBubble.jellies)
        {
            if (jelly.GetComponent<JellyController>().Type == colorType)
            {
                temp.Add(jelly);
            }
        }

        // Di chuyển các jelly có màu tương ứng sang mergeBubble
        foreach (var jelly in temp)
        {
            jelly.GetComponent<JellyController>().PlayMoveAnimate(_mergeBubble.gameObject, jellyMoveDuration);
            _mergeBubble.jellies.Add(jelly);
            _breakBubble.jellies.Remove(jelly);
        }

        // Xóa danh sách jellies sau khi đã di chuyển
        temp.Clear();

        // Tìm các jelly có màu khác từ mergeBubble để di chuyển ngược lại sang breakBubble
        foreach (var jelly in _mergeBubble.jellies)
        {
            if (jelly.GetComponent<JellyController>().Type != colorType)
            {
                temp.Add(jelly);
            }
        }

        // Di chuyển các jelly có màu khác về breakBubble
        foreach (var jelly in temp)
        {
            jelly.GetComponent<JellyController>().PlayMoveAnimate(_breakBubble.gameObject, jellyMoveDuration);
            _breakBubble.jellies.Add(jelly);
            _mergeBubble.jellies.Remove(jelly);
        }

        if (moveBack)
        {
            int priorityColor = temp[^1].GetComponent<JellyController>().Type;
            temp.Clear();
            foreach (var jelly in _breakBubble.jellies)
            {
                if (jelly.GetComponent<JellyController>().Type != priorityColor)
                {
                    temp.Add(jelly);
                }
            }
            foreach (var jelly in temp)
            {
                jelly.GetComponent<JellyController>().PlayMoveAnimate(_mergeBubble.gameObject, jellyMoveDuration);
                _mergeBubble.jellies.Add(jelly);
                _breakBubble.jellies.Remove(jelly);
            }
        }

        // Cập nhật collider sau khi hoàn tất di chuyển các jelly
        foreach (var jelly in _mergeBubble.jellies)
        {
            jelly.GetComponent<JellyController>().ReduceColliderRadius(_radiusReduceAmount * _mergeBubble.jellies.Count);
        }

        foreach (var jelly in _breakBubble.jellies)
        {
            jelly.GetComponent<JellyController>().ReduceColliderRadius(_radiusReduceAmount * _breakBubble.jellies.Count);
        }
    }

    void TradeBubbleWithTwoSameColor(int mergeId, int breakId, List<int> colorTypes, bool moveBack)
    {
        // Lấy thông tin các bubble từ mergeId và breakId
        _mergeBubble = _bubbles[mergeId].GetComponent<BubbleController>();
        _breakBubble = _bubbles[breakId].GetComponent<BubbleController>();

        List<GameObject> tempColor = new List<GameObject>();

        // Ngắt kết nối các jelly màu firstColorType từ breakBubble và secondColorType từ mergeBubble
        _breakBubble.DisConnectToType(colorTypes[0], true);
        _mergeBubble.DisConnectToType(colorTypes[1], true);

        // Di chuyển jelly có màu firstColorType từ breakBubble sang mergeBubble
        foreach (var jelly in _breakBubble.jellies)
        {
            if (jelly.GetComponent<JellyController>().Type == colorTypes[0])
            {
                tempColor.Add(jelly);
            }
        }

        foreach (var jelly in tempColor)
        {
            jelly.GetComponent<JellyController>().PlayMoveAnimate(_mergeBubble.gameObject, jellyMoveDuration);
            _mergeBubble.jellies.Add(jelly);
            _breakBubble.jellies.Remove(jelly);
        }

        // Xóa danh sách jellies sau khi đã di chuyển từ breakBubble
        tempColor.Clear();

        foreach (var jelly in _mergeBubble.jellies)
        {
            if (jelly.GetComponent<JellyController>().Type == colorTypes[1])
            {
                tempColor.Add(jelly);
            }
        }

        // Di chuyển jelly có màu secondColorType từ mergeBubble sang breakBubble
        foreach (var jelly in tempColor)
        {
            jelly.GetComponent<JellyController>().PlayMoveAnimate(_breakBubble.gameObject, jellyMoveDuration);
            _breakBubble.jellies.Add(jelly);
            _mergeBubble.jellies.Remove(jelly);
        }

        if (moveBack)
        {
            int priorityColor = tempColor[^1].GetComponent<JellyController>().Type;
            tempColor.Clear();
            foreach (var jelly in _breakBubble.jellies)
            {
                if (jelly.GetComponent<JellyController>().Type != priorityColor)
                {
                    tempColor.Add(jelly);
                }
            }
            foreach (var jelly in tempColor)
            {
                jelly.GetComponent<JellyController>().PlayMoveAnimate(_mergeBubble.gameObject, jellyMoveDuration);
                _mergeBubble.jellies.Add(jelly);
                _breakBubble.jellies.Remove(jelly);
            }
        }

        // Cập nhật collider sau khi di chuyển
        foreach (var jelly in _mergeBubble.jellies)
        {
            jelly.GetComponent<JellyController>().ReduceColliderRadius(_radiusReduceAmount * _mergeBubble.jellies.Count);
        }

        foreach (var jelly in _breakBubble.jellies)
        {
            jelly.GetComponent<JellyController>().ReduceColliderRadius(_radiusReduceAmount * _breakBubble.jellies.Count);
        }
    }

    void DoneBubble(DoneBubbleEvent e)
    {
        _bubbles[e.id].GetComponent<BubbleController>().PrevDone(e.maxNumb);
        _bubbles[e.id].GetComponent<BubbleController>().IsDone();
    }

    private void Update()
    {
        _dropDelay -= Time.deltaTime;
        if (Input.GetMouseButtonDown(0))
        {
            if (_dropBubble != null || !_boosterIsOn || !_gameSceneModel.IsDropping)
            {
                if (!IsPointerOverButton())
                {
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, mapLayer);
                    if (hit.collider != null && _dropBubble != null && _dropDelay <= 0)
                    {
                        _isDragging = true;
                        _mouseMovementSystem.MouseClick(_dropBubble.transform, offset, extents, leftBorder, rightBorder);
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (_dropBubble != null || !_boosterIsOn)
            {
                if (!IsPointerOverButton() && _isDragging)
                {
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, mapLayer);
                    if (vibrationIsAvailable) Vibration.VibratePop();
                    if (hit.collider != null && _dropBubble != null && _dropDelay <= 0)
                    {
                        _isDragging = false;
                        _dropBubbleSystem.DropBubble(ref _dropBubble, _dropBubbleRb);
                    }
                }
            }
        }

        if (_isDragging)
        {
            _mouseMovementSystem.MouseDrag(_dropBubble.transform, offset, extents, leftBorder, rightBorder); // Cập nhật vị trí bubble theo chuột
        }
        if (!_boosterIsOn) return;
        if (Input.GetMouseButtonDown(0))
        {
            if (_boosterType == 0 || _boosterType == 1)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, clickableLayer);
                if (vibrationIsAvailable) Vibration.VibratePop();
                if (hit.collider != null)
                {
                    hit.collider.gameObject.GetComponent<BubbleController>().BoosterClickOn();
                }
            }
            else
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                this.SendCommand(new PlayFirecrackerAniCommand
                {
                    mousePos = mousePosition,
                });
            }
        }

    }

    public bool IsPointerOverButton()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.GetComponent<Button>() != null)
            {
                // The pointer is over a button
                return true;
            }
        }

        // The pointer is not over any buttons
        return false;
    }

    void OnDestroy()
    {
        foreach (var bubble in _bubbles)
        {
            Destroy(bubble);
        }
    }
    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
