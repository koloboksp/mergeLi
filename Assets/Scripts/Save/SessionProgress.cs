using System;
using System.Collections.Generic;

[Serializable]
public class SessionProgress
{
    public SessionFieldProgress Field;
    public SessionCastleProgress Castle;
    public List<SessionBuffProgress> Buffs = new();
    public int Score;
       
    public bool IsValid()
    {
        if (string.IsNullOrEmpty(Castle.Id))
            return false;

        return true;
    }
}