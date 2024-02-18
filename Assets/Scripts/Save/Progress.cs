using System;
using System.Collections.Generic;

[Serializable]
public class Progress
{
    public List<string> CompletedCastles = new List<string>();
    public int BestSessionScore;
    public int Coins;
    public List<TutorialProgress> Tutorials = new();
}