public enum MinigameType
{
    Mash
}

public static class MinigameFactory
{
    public static IMinigame Create(MinigameType type, MinigameManager manager)
    {
        switch (type)
        {
            case MinigameType.Mash:
                return new MashMinigame(manager);
            default:
                throw new System.Exception($"Minigame de tip {type} nu exista!");
        }
    }
}