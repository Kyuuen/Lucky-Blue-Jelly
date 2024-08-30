using QFramework;

public class BlueJellyClone : Architecture<BlueJellyClone>
{
    protected override void Init()
    {
        RegisterModel<IGameSceneModel>(new GameSceneModel());
        RegisterModel<IPlayerPrefModel>(new PlayerPrefModel());

        RegisterSystem<IBoosterSystem>(new BoosterSystem());
        RegisterSystem<IDropBubbleSystem>(new DropBubbleSystem());
        RegisterSystem<IGoldSystem>(new GoldSystem());
        RegisterSystem<ILevelStartSpawnSystem>(new LevelStartSpawnSystem());
        RegisterSystem<IMergeBubbleSystem>(new MergeBubbleSystem());
        RegisterSystem<IMouseMovementSystem>(new MouseMovementSystem());
        RegisterSystem<IScoreSystem>(new ScoreSystem());
        RegisterSystem<ISpawnJellySystem>(new SpawnJellySystem());
        RegisterSystem<IUISystem>(new UIEventSystem());
        RegisterSystem<IEventCenterSystem>(new EventCenterSystem());

        RegisterUtility<IStorage>(new PlayerPrefsStorage());
    }
}
