namespace PointBlank.Api
{
    public class API_MACHINE_DATA_REQ : ApiPacketReader
    {
        public override void ReadImplement()
        {
            byte hardwareLength = ReadByte();
            byte ipLength = ReadByte();
            byte macLength = ReadByte();
            client.machine = new MachineModel();
            client.machine.IpAddress = ReadString(ipLength);
            client.machine.MacAddress = ReadString(macLength);
            client.machine.HardwareId = ReadString(hardwareLength);
        }

        public override void RunImplement()
        {
            MachineModel machine = client.machine;
            if (machine != null)
            {
                if (machine.IpAddress == "" || machine.MacAddress == "" || machine.HardwareId == "")
                {
                    Logger.Warning($" [API_MACHINE_DATA_REQ] MAchine informations invalid! HardwareId: {machine.HardwareId} MacAddress: {machine.MacAddress} IpAddress: {machine.IpAddress}");
                    client.Close();
                    return;
                }
                Logger.Warning($" MACHINE INFO: HardwareId: {machine.HardwareId} MacAddress: {machine.MacAddress} IpAddress: {machine.IpAddress}");
                client.SendPacket(new API_SETTINGS_INFO_ACK());
            }
            else
            {
                client.Close();
            }
        }
    }
}