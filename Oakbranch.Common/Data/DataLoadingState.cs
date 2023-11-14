using System;

namespace Oakbranch.Common.Data
{
    public enum DataLoadingState
    {
        /// <summary>
        /// Related property(-ies) has not been deserialized yet.
        /// </summary>
        NotLoaded = 0,
        /// <summary>
        /// Data and related property(-ies) are synchronized.
        /// </summary>
        Loaded = 1,
        /// <summary>
        /// Data has been modified but not serialized yet.
        /// </summary>
        Modified = 2,
        /// <summary>
        /// Data is being processed (either serialized or deserialized).
        /// </summary>
        BeingSynchronized = 3
    }
}
