namespace PointBlank.Game
{
    public class PROTOCOL_LOBBY_PLAYER_EQUIPMENTS_ACK : GamePacketWriter
    {
        private PlayerEquipedItems equipments;
        public PROTOCOL_LOBBY_PLAYER_EQUIPMENTS_ACK(PlayerEquipedItems equipments)
        {
            this.equipments = equipments;
        }

        public override void Write()
        {
            WriteH(3100);
            if (equipments != null)
            {
                WriteD(equipments.primary);
                WriteD(equipments.secondary);
                WriteD(equipments.melee);
                WriteD(equipments.grenade);
                WriteD(equipments.special);
                WriteD(equipments.red);
                WriteD(equipments.blue);
                WriteD(equipments.helmet);
                WriteD(equipments.beret);
                WriteD(equipments.dino);
            }
            else
            {
                WriteD(0);
                WriteD(601002003);
                WriteD(702001001);
                WriteD(803007001);
                WriteD(904007002);
                WriteD(1001001005);
                WriteD(1001002006);
                WriteD(1102003001);
                WriteD(0);
                WriteD(1006003041);
            }
            WriteD(0); //Count de writeD
        }
    }
}