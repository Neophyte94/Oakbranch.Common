using System;

namespace Oakbranch.Common.Data
{
    public interface ILazyRefLoader<out T>
    {
        bool HasLoaded(object entity);
        T Load(object entity);
    }
}
