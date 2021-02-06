using PointBlank.Api;
using System;
using System.Globalization;
using System.Threading;

namespace PointBlank.Game
{
    public class PROTOCOL_LOBBY_ENTER_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                if (player == null)
                {
                    return;
                }
                DateTime now = DateTime.Now;
                if ((now - client.SessionDate).TotalMilliseconds < 1000)
                {
                    Logger.Warning($" [Game] Connection destroyed on suspicion of modified client. (Time Lobby) IPAddress: {client.GetIPAddress()}");
                    client.Close(0, true);
                    return;
                }
                if (player.channelId >= 0)
                {
                    Channel channel = player.GetChannel();
                    if (channel != null)
                    {
                        channel.AddPlayer(player.session);
                    }
                }
                Room room = player.room;
                if (room != null)
                {
                    if (player.slotId >= 0 && room.state >= RoomStateEnum.Loading && room.slots[player.slotId].state >= SlotStateEnum.LOAD)
                    {
                        client.SendCompletePacket(PackageDataManager.LOBBY_ENTER_PAK);
                        player.lastLobbyEnter = now;
                        return;
                    }
                    else
                    {
                        room.RemovePlayer(player, false);
                    }
                }
                player.SyncPlayerToFriends(false);
                player.SyncPlayerToClanMembers();
                client.SendCompletePacket(PackageDataManager.LOBBY_ENTER_PAK);
                player.lastLobbyEnter = now;
                GetXmasReward(player);

                if (!player.showBoxWelcome && player.nickname.Length > 0)
                {
                    player.showBoxWelcome = true;
                    Thread.Sleep(2250);
                    client.SendPacket(new LOBBY_CHATTING_PAK("Project Blackout", player.GetSessionId(), 5, false, $" [{GameManager.SocketSessions.Count} Online] Project Blackout Brazil"));
                    if (player.pccafe > 0)
                    {
                        DateTime pccafeDate = DateTime.ParseExact(player.pccafeDate.ToString(), "yyMMddHHmm", CultureInfo.InvariantCulture);
                        client.SendPacket(new LOBBY_CHATTING_PAK("Project Blackout", player.GetSessionId(), 5, false, $" [VIP {(player.pccafe == 1 ? "BASIC" : "PREMIUM")} {(pccafeDate - DateTime.Now).Days}D] Aproveite nossa loja vip feita especialmente para você!"));
                    }
                }
                ApiManager.SendPacketToAllClients(new API_USER_LOBBY_ENTER_ACK(player));
                ApiManager.SendPacketToAllClients(new API_SERVER_INFO_ACK());
                Thread.Sleep(3000);
                if (!player.firstLobbyEnter)
                {
                    player.firstLobbyEnter = true;
                    if (!client.PacketGetRoomList || !player.loadedShop)
                    {
                        Logger.Warning($" [LOBBY_ENTER_REQ] Connection destroyed because you did not request the list of rooms or list of shop. Login: {player.login} PlayerId: {player.playerId} IPAddress ({client.GetIPAddress()})");
                        client.Close();
                    }
                    //player.CheckRoomList();
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }

        public void GetXmasReward(Account player)
        {
            EventXmasModel eventRunning = EventXmasSyncer.GetRunningEvent();
            if (eventRunning != null)
            {
                PlayerEvent playerEvents = player.events;
                int date = int.Parse(DateTime.Now.ToString("yyMMddHHmm"));
                if (playerEvents != null && !(playerEvents.LastXmasRewardDate > eventRunning.startDate && playerEvents.LastXmasRewardDate <= eventRunning.endDate) && player.ExecuteQuery($"UPDATE player_events SET last_xmas_reward_date='{(int)date}' WHERE player_id='{player.playerId}'"))
                {
                    playerEvents.LastXmasRewardDate = date;
                    player.SendPacket(new PROTOCOL_INVENTORY_ITEM_CREATE_ACK(1, player, new ItemsModel(702001024, 1, "Alcaçuz (Evento XMAS)", 1, 100)));
                }
            }
        }
    }
}