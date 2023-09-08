using System;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Oakbranch.Common.Utility
{
    public static class ExceptionUtility
    {
        #region Static members

        private static readonly MethodInfo ExceptionPreserveStackMethod;
            

        #endregion

        #region Static constructor

        static ExceptionUtility()
        {
            ExceptionPreserveStackMethod = typeof(Exception)
                .GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        #endregion

        #region Static methods

        public static void PreserveStackTrace(this Exception ex)
        {
            ExceptionPreserveStackMethod.Invoke(ex, null);
        }

        public static void RethrowWithOriginalStack(this Exception ex)
        {
            ExceptionDispatchInfo exInfo = ExceptionDispatchInfo.Capture(ex);
            exInfo.Throw();
        }

        #endregion
    }
}
