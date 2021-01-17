namespace PointBlank.Game
{
    public class PROTOCOL_A_3902_REQ : GamePacketReader
    {
        public override void ReadImplement()
        {
        }

        public override void RunImplement()
        {
            if (client == null || client.SessionPlayer == null)
                return;
            try
            {
                //Ativa quando usa "/ROOMDEST"
                Logger.Warning("[3902]");
            }
            catch
            {
            }
        }
    }
}