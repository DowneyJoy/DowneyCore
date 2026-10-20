using AT.Core.Runtime.Basic;
using AT.Core.Runtime.Utils;

namespace AT.Core.Runtime.Manager
{
    public class CoroutineHandler : AutoSingleton<CoroutineHandler>,IPersistence
    {
        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}