﻿using System;

namespace Save
{
    [Serializable]
    public class GiftProgress
    {
        public string Id;
        public int TimesCollected;
        public long LastCollectedTimestamp;
    }
}