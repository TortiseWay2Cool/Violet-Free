using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace VioletTemplate.Main.Extentions
{
    internal class Networking : IPhotonPeerListener
    {
        public static string[] GetValidUserIDs()
        {
            return null;
        }

        public void DebugReturn(DebugLevel level, string message)
        {
            // Nothin
        }
        public void OnEvent(EventData eventData)
        {
            if (eventData.Code == 116)
            {

            }
        }
        public void OnOperationResponse(OperationResponse operationResponse)
        {
            switch (operationResponse.OperationCode)
            {
                case 230:
                {
                    // Joined Room
                    break;
                }
            }
        }
        public void OnStatusChanged(StatusCode statusCode)
        {
            // Nothin
        }
    }
}
