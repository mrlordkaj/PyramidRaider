using Microsoft.Xna.Framework.GamerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
#if WINDOWS_PHONE
using Microsoft.Phone.Info;
using Microsoft.Phone.Tasks;
#endif

namespace OpenitvnGame.Helpers
{
    public interface IFeedbackCaller
    {
        void Review();
        void Feedback();
    }
}
