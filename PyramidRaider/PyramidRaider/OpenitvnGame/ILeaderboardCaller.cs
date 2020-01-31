using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenitvnGame
{
    interface ILeaderboardCaller
    {
        void OnSubmitSuccess();
        void OnSubmitFailed();

        void OnGetRankSuccess(int rank);
        void OnGetRankFailed();

        void OnView7Success(StreamReader reader);
        void OnView7Failed();

        void OnViewAllSuccess(StreamReader reader);
        void OnViewAllFailed();

        void OnRemoveSuccess();
        void OnRemoveFailed();
    }
}
