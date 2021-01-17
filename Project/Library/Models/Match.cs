using PointBlank.Game;
using System.Collections.Generic;

namespace PointBlank
{
    public class Match
    {
        public Clan clan;
        public int formação;
        public int serverId;
        public int channelId;
        public int matchId = -1;
        public int leader;
        public int friendId;
        public SlotMatch[] slots = new SlotMatch[8];
        public MatchStateEnum state = MatchStateEnum.Ready;
        public Match(Clan clan)
        {
            this.clan = clan;
            for (int i = 0; i < 8; i++)
            {
                slots[i] = new SlotMatch(i);
            }
        }

        public bool GetSlot(int slotId, out SlotMatch slot)
        {
            lock (slots)
            {
                slot = null;
                if (slotId >= 0 && slotId < 16)
                {
                    slot = slots[slotId];
                }
                return slot != null;
            }
        }

        public void SetNewLeader(int leader, int oldLeader)
        {
            lock (slots)
            {
                if (leader == -1)
                {
                    for (int i = 0; i < formação; i++)
                    {
                        if (i != oldLeader && slots[i].playerId > 0)
                        {
                            this.leader = i;
                            break;
                        }
                    }
                }
                else
                {
                    this.leader = leader;
                }
            }
        }

        public bool AddPlayer(Account player)
        {
            lock (slots)
            {
                for (int i = 0; i < formação; i++)
                {
                    SlotMatch slot = slots[i];
                    if (slot.playerId == 0 && slot.state == 0)
                    {
                        slot.playerId = player.playerId;
                        slot.state = SlotMatchStateEnum.Normal;
                        player.match = this;
                        player.matchSlot = i;
                        player.status.UpdateClanMatch((byte)friendId);
                        player.SyncPlayerToClanMembers();
                        return true;
                    }
                }
            }
            return false;
        }

        public Account GetPlayerBySlot(SlotMatch slot)
        {
            try
            {
                long id = slot.playerId;
                return id > 0 ? AccountManager.GetAccount(id, true) : null;
            }
            catch
            {
                return null;
            }
        }

        public Account GetPlayerBySlot(int slotId)
        {
            try
            {
                long id = slots[slotId].playerId;
                return id > 0 ? AccountManager.GetAccount(id, true) : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gera uma lista das contas dos jogadores, com uma excessão de slot.
        /// </summary>
        /// <param name="exception">Slot do jogador</param>
        /// <returns></returns>
        public List<Account> GetAllPlayers(int exception)
        {
            List<Account> list = new List<Account>();
            lock (slots)
            {
                for (int i = 0; i < 8; i++)
                {
                    long id = slots[i].playerId;
                    if (id > 0 && i != exception)
                    {
                        Account p = AccountManager.GetAccount(id, true);
                        if (p != null)
                        {
                            list.Add(p);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Gera uma lista das contas dos jogadores.
        /// </summary>
        /// <returns></returns>
        public List<Account> GetAllPlayers()
        {
            List<Account> list = new List<Account>();
            lock (slots)
            {
                for (int i = 0; i < 8; i++)
                {
                    long id = slots[i].playerId;
                    if (id > 0)
                    {
                        Account p = AccountManager.GetAccount(id, true);
                        if (p != null)
                        {
                            list.Add(p);
                        }
                    }
                }
            }
            return list;
        }

        public void SendPacketToPlayers(GamePacketWriter packet)
        {
            List<Account> players = GetAllPlayers();
            if (players.Count == 0)
            {
                return;
            }

            byte[] data = packet.GetCompleteBytes("Match.SendPacketToPlayers(SendPacket)");
            for (int i = 0; i < players.Count; i++)
            {
                Account pR = players[i];
                pR.SendCompletePacket(data);
            }
        }

        public void SendPacketToPlayers(GamePacketWriter packet, int exception)
        {
            List<Account> players = GetAllPlayers(exception);
            if (players.Count == 0)
            {
                return;
            }

            byte[] data = packet.GetCompleteBytes("Match.SendPacketToPlayers(SendPacket,int)");
            for (int i = 0; i < players.Count; i++)
            {
                Account pR = players[i];
                pR.SendCompletePacket(data);
            }
        }

        public Account GetLeader()
        {
            try
            {
                return AccountManager.GetAccount(slots[leader].playerId, true);
            }
            catch
            {
                return null;
            }
        }

        public int GetServerInfo()
        {
            return channelId + (serverId * 10);
        }

        public int GetCountPlayers()
        {
            lock (slots)
            {
                int count = 0;
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].playerId > 0)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        private void BaseRemovePlayer(Account p)
        {
            lock (slots)
            {
                if (!GetSlot(p.matchSlot, out SlotMatch slot))
                {
                    return;
                }
                if (slot.playerId == p.playerId)
                {
                    slot.playerId = 0;
                    slot.state = SlotMatchStateEnum.Empty;
                }
            }
        }

        public bool RemovePlayer(Account p)
        {
            Channel ch = ServersManager.GetChannel(channelId);
            if (ch == null)
            {
                return false;
            }
            BaseRemovePlayer(p);
            if (GetCountPlayers() == 0)
            {
                ch.RemoveMatch(matchId);
            }
            else
            {
                if (p.matchSlot == leader)
                {
                    SetNewLeader(-1, -1);
                }
                using (CLAN_WAR_REGIST_MERCENARY_PAK packet = new CLAN_WAR_REGIST_MERCENARY_PAK(this))
                {
                    SendPacketToPlayers(packet);
                }
            }
            p.matchSlot = -1;
            p.match = null;
            return true;
        }
    }
}