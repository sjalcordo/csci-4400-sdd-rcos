using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voting
{
    public class VotingButton : MonoBehaviour
    {
        public void SendApproval()
        {
            Sockets.ServerUtil.manager.SendEvent("approval");
        }

        public void SendDisapproval()
        {
            Sockets.ServerUtil.manager.SendEvent("disapproval");
        }
    }
}

