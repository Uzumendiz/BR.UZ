using System;

namespace PointBlank.Game
{
    public class PROTOCOL_VOTEKICK_START_REQ : GamePacketReader
    {
        private int motive, slotId;
        public override void ReadImplement()
        {
            slotId = ReadByte();
            motive = ReadByte();
            //motive 0=NoManner|1=IllegalProgram|2=Abuse|3=ETC
        }

        public override void RunImplement()
        {
            try
            {
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (slotId < 0 || slotId > 15 || motive < 0 || motive > 3 || room == null || room.state != RoomStateEnum.Battle || player.slotId == slotId)
                {
                    return;
                }
                Slot slot = room.GetSlot(player.slotId);
                if (slot != null && slot.state == SlotStateEnum.BATTLE && room.slots[slotId].state == SlotStateEnum.BATTLE)
                {
                    if (!Settings.VoteKickActive)
                    {
                        client.SendPacket(new LOBBY_CHATTING_PAK("[VoteKick]", player.GetSessionId(), 0, true, "O sistema de votação está desativado temporariamente."));
                        return;
                    }
                    room.GetPlayingPlayers(true, out int redPlayers, out int bluePlayers);
                    if (redPlayers == 1 && bluePlayers == 1)
                    {
                        client.SendCompletePacket(PackageDataManager.VOTEKICK_CHECK_ERROR_0x800010E2_PAK);
                        return;
                    }
                    if (player.rankId < Settings.MinRankStartVoteKick && !player.HaveGMLevel())
                    {
                        client.SendCompletePacket(PackageDataManager.VOTEKICK_CHECK_ERROR_0x800010E4_PAK);
                        return;
                    }
                    else if (room.vote.Timer != null)
                    {
                        client.SendCompletePacket(PackageDataManager.VOTEKICK_CHECK_ERROR_0x800010E0_PAK);
                        return;
                    }
                    else if (slot.voteCounts > Settings.MaxStartVoteKick)
                    {
                        client.SendPacket(new LOBBY_CHATTING_PAK("[VoteKick]", player.GetSessionId(), 0, true, $"Não é possivel abrir mais de {Settings.MaxStartVoteKick} votação por partida."));
                        client.SendCompletePacket(PackageDataManager.VOTEKICK_CHECK_ERROR_0x800010E1_PAK);
                        return;
                    }
                    else if (slot.nextVoteDate > DateTime.Now)
                    {
                        client.SendCompletePacket(PackageDataManager.VOTEKICK_CHECK_ERROR_0x800010E1_PAK);
                        return;
                    }
                    client.SendCompletePacket(PackageDataManager.VOTEKICK_CHECK_SUCCESS_PAK);
                    slot.nextVoteDate = DateTime.Now.AddMinutes(Settings.NextVoteKickMinutes);
                    slot.voteCounts++;
                    room.votekick = new VoteKick(slot.Id, slotId)
                    {
                        motive = motive
                    };
                    ChargeVoteKickArray(room);
                    using (VOTEKICK_START_PAK packet = new VOTEKICK_START_PAK(room.votekick))
                    {
                        room.SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0, player.slotId, slotId);
                    }
                    room.StartVote();
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
        /// <summary>
        /// Configura a array com os jogadores em partida.
        /// </summary>
        /// <param name="room"></param>
        private void ChargeVoteKickArray(Room room)
        {
            for (int i = 0; i < 16; i++)
            {
                Slot slot = room.slots[i];
                room.votekick.TotalArray[i] = slot.state == SlotStateEnum.BATTLE;
            }
        }
    }
}