﻿#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using UnityEngine.Events;

namespace HeathenEngineering.SteamworksIntegration
{
    [System.Serializable]
    public class GameServerSetEvent : UnityEvent<LobbyGameServer>
    { }
}
#endif