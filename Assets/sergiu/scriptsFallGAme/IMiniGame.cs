using System;

public interface IMinigame
{
    void StartMinigame(Action<bool> onComplete);
    void UpdateMinigame();
    void EndMinigame(bool success);
}