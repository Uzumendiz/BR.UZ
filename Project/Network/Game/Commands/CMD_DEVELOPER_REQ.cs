namespace PointBlank
{
    public class CMD_DEVELOPER_REQ : PacketCommand
    {
        private string command;
        private byte type;
        public CMD_DEVELOPER_REQ(string command, byte type)
        {
            this.command = command;
            this.type = type;
        }

        public override void RunImplement()
        {
            if (type == 1)
            {
                if (room != null)
                {
                    if (room.IsPreparing())
                    {
                        room.EndBattle(room.IsBotMode(), room.GetWinnerTeam());
                        response = $"Você finalizou a partida da sala {room.roomId + 1}";
                    }
                    else
                    {
                        response = "Não foi possivel finalizar a partida no momento.";
                    }
                }
                else
                {
                    response = "Você precisa estar presente em uma sala.";
                }
            }
            else if (type == 2) //StageType
            {
                int stageType = int.Parse(command.Substring(9));
                if (room != null)
                {
                    room.mode = (RoomTypeEnum)stageType;
                    room.UpdateRoomInfo();
                    response = $"Você alterou o modo da sala. Mode: {room.mode}";
                }
                else
                {
                    response = "Você precisa estar presente em uma sala.";
                }
            }
            else if (type == 3) //SpecialType
            {
                int special = int.Parse(command.Substring(12));
                if (room != null)
                {
                    room.modeSpecial = (RoomModeSpecial)special;
                    room.UpdateRoomInfo();
                    response = $"Você alterou o modo especial da sala. ModeSpecial: {room.modeSpecial}";
                }
                else
                {
                    response = "Você precisa estar presente em uma sala.";
                }
            }
            else if (type == 4) //WeaponsFlag
            {
                int flags = int.Parse(command.Substring(11));
                if (room != null)
                {
                    room.weaponsFlag = (byte)flags;
                    room.UpdateRoomInfo();
                    response = $"Você alterou a flag dos equipamentos da sala. WeaponsFlag: {(RoomWeaponsFlag)flags}";
                }
                else
                {
                    response = "Você precisa estar presente em uma sala.";
                }
            }
        }
    }
}