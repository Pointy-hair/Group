﻿namespace Traffk.Utility
{
    public interface ICorrelationIdFactory
    {
        string Key { get; set; }
        string Create();
    }
}
