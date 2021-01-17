using System;

namespace PointBlank.Game
{
    public class PROTOCOL_VOTEKICK_UPDATE_REQ : GamePacketReader
    {
        private byte type;
        public override void ReadImplement()
        {
            type = ReadByte();
        }

        public override void RunImplement()
        {
            try
            {
                Logger.DebugPacket(GetType().Name, $"KickType: {type}");
                Account player = client.SessionPlayer;
                Room room = player != null ? player.room : null;
                if (room == null || room.state != RoomStateEnum.Battle || room.vote.Timer == null || room.votekick == null || !room.GetSlot(player.slotId, out Slot slot) || slot.state != SlotStateEnum.BATTLE)
                {
                    return;
                }
                VoteKick vote = room.votekick;
                if (vote.votes.Contains(player.slotId))
                {
                    client.SendCompletePacket(PackageDataManager.VOTEKICK_UPDATE_RESULT_ERROR_PAK);
                    return;
                }
                lock (vote.votes)
                {
                    vote.votes.Add(slot.Id);
                }
                if (type == 0)
                {
                    vote.kikar++;
                    if (slot.teamId == vote.victimIdx % 2)
                    {
                        vote.allies++;
                    }
                    else
                    {
                        vote.enemys++;
                    }
                }
                else
                {
                    vote.deixar++;
                }
                if (vote.votes.Count >= vote.GetInGamePlayers())
                {
                    room.vote.Timer = null;
                    room.VotekickResult();
                }
                else
                {
                    using (VOTEKICK_UPDATE_PAK packet = new VOTEKICK_UPDATE_PAK(vote))
                    {
                        room.SendPacketToPlayers(packet, SlotStateEnum.BATTLE, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                PacketLog(ex);
            }
        }
    }
}