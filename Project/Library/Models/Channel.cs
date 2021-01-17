using System;
using System.Collections.Generic;
using System.Linq;

namespace PointBlank
{
    public class Channel
    {
        public int id;
        public int type;
        public int serverId;
        public string announce = "BR.UZ Server";
        public List<PlayerSession> players = new List<PlayerSession>();
        public List<Room> rooms = new List<Room>();
        public List<Match> matchs = new List<Match>();
        private DateTime LastRoomsSync = DateTime.Now;
        public PlayerSession GetPlayer(int sessionId)
        {
            lock (players)
            {
                //for (int i = 0; i < players.Count; i++)
                //{
                //    PlayerSession playerSession = players[i];
                //    if (playerSession.sessionId == sessionId)
                //    {
                //        return playerSession;
                //    }
                //}
                return players.Where(x => x.sessionId == sessionId).FirstOrDefault();
            }
        }

        public bool AddPlayer(PlayerSession playerSession)
        {
            lock (players)
            {
                try
                {
                    if (!players.Contains(playerSession))
                    {
                        players.Add(playerSession);
                        ServersManager.UpdateServerPlayers();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }
            return false;
        }

        public void RemoveMatch(int matchId)
        {
            lock (matchs)
            {
                for (int i = 0; i < matchs.Count; i++)
                {
                    if (matchId == matchs[i].matchId)
                    {
                        matchs.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void AddMatch(Match match)
        {
            lock (matchs)
            {
                if (!matchs.Contains(match))
                {
                    matchs.Add(match);
                }
            }
        }

        /// <summary>
        /// Adiciona uma sala no canal. Proteção Thread-Safety.
        /// </summary>
        /// <param name="room">Sala</param>
        public bool AddRoom(Room room)
        {
            lock (rooms)
            {
                try
                {
                    rooms.Add(room);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// Limpa as salas que a quantidade de jogadores for 0. Proteção Thread-Safety.
        /// </summary>
        public void RemoveEmptyRooms()
        {
            lock (rooms)
            {
                try
                {
                    if ((DateTime.Now - LastRoomsSync).TotalSeconds >= Settings.EmptyRoomRemovalInterval)
                    {
                        LastRoomsSync = DateTime.Now;
                        for (int i = 0; i < rooms.Count; i++)
                        {
                            Room room = rooms[i];
                            if (room.GetAllPlayersCount() < 1)
                            {
                                rooms.RemoveAt(i--);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }
        }
        public Match GetMatch(int matchId)
        {
            lock (matchs)
            {
                //for (int i = 0; i < matchs.Count; i++)
                //{
                //    Match match = matchs[i];
                //    if (match.matchId == matchId)
                //    {
                //        return match;
                //    }
                //}
                return matchs.Where(x => x.matchId == matchId).FirstOrDefault();
            }
        }
        public Match GetMatch(int id, int clan)
        {
            lock (matchs)
            {
                //for (int i = 0; i < matchs.Count; i++)
                //{
                //    Match match = matchs[i];
                //    if (match.friendId == id && match.clan.id == clan)
                //    {
                //        return match;
                //    }
                //}
                return matchs.Where(x => x.friendId == id && x.clan.id == clan).FirstOrDefault();
            }
        }
        /// <summary>
        /// Procura uma sala no canal. (Proteção Thread-Safety)
        /// </summary>
        /// <param name="roomId">Id da sala</param>
        /// <returns></returns>
        public Room GetRoom(int roomId)
        {
            lock (rooms)
            {
                //for (int i = 0; i < rooms.Count; i++)
                //{
                //    Room room = rooms[i];
                //    if (room.roomId == roomId)
                //    {
                //        return room;
                //    }
                //}
                return rooms.Where(x => x.roomId == roomId).FirstOrDefault();
            }
        }
        /// <summary>
        /// Gera uma lista de contas que não estão em uma sala, que possuem um apelido, sem utilizar a Database. Proteção Thread-Safety.
        /// </summary>
        /// <returns></returns>
        public List<Account> GetWaitPlayers()
        {
            List<Account> list = new List<Account>();
            lock (players)
            {
                foreach (PlayerSession session in players)
                {
                    Account player = AccountManager.GetAccount(session.playerId, true);
                    if (player != null && player.room == null && player.nickname.Length >= Settings.NickMinLength && player.nickname.Length <= Settings.NickMaxLength)
                    {
                        list.Add(player);
                    }
                }
            }
            return list;
        }

        public void SendPacketToWaitPlayers(GamePacketWriter packet)
        {
            List<Account> players = GetWaitPlayers();
            if (players.Count == 0)
            {
                return;
            }
            byte[] data = packet.GetCompleteBytes("Channel.SendPacketToWaitPlayers");
            for (int i = 0; i < players.Count; i++)
            {
                players[i].SendCompletePacket(data);
            }
        }

        public bool RemovePlayer(Account player)
        {
            bool result = false;
            try
            {
                player.channelId = -1;
                if (player.session != null)
                {
                    lock (players)
                    {
                        result = players.Remove(player.session);
                    }
                    if (result)
                    {
                        ServersManager.UpdateServerPlayers();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            return result;
        }
    }
}