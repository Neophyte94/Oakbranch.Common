using System;
using System.Collections.Generic;

namespace Oakbranch.Common.Data
{
    public interface ILazySetLoader<T>
    {
        bool HasLoaded(object entity);
        IList<T> Load(object entity);
        int GetPartnersCount(object entity);
    }
}
