public enum MinigameType
{
    Mash,
    Chase,
    StayAwake
}

public static class MinigameFactory
{
   public static IMinigame Create(MinigameType type, MinigameManager manager)
    {
        switch (type)
        {
            case MinigameType.Mash:
                return new MashMinigame(manager);
            case MinigameType.Chase:
                return new ChaseMinigame(manager);
            case MinigameType.StayAwake:
                return new StayAwakeMinigame(manager);
            default:
                throw new System.Exception($"Minigame de tip {type} nu exista!");
        }
    }
}