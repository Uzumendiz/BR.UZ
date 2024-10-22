﻿namespace PointBlank.Api
{
    public class API_LOGIN_ADMIN_REQ : ApiPacketReader
    {
        private string login;
        private string password;
        public override void ReadImplement()
        {
            int loginLength = ReadByte();
            int passwordLength = ReadByte();
            byte hardwareLength = ReadByte();
            byte ipLength = ReadByte();
            byte macLength = ReadByte();
            login = ReadString(loginLength);
            password = ReadString(passwordLength);
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
                    Logger.Warning($" [API_MACHINE_DATA_REQ] Machine informations invalid! HardwareId: {machine.HardwareId} MacAddress: {machine.MacAddress} IpAddress: {machine.IpAddress}");
                    client.SendPacket(new API_LOGIN_ADMIN_RESULT_ACK(null, 4));
                    client.Close();
                    return;
                }
                Logger.Warning($" MACHINE INFO: HardwareId: {machine.HardwareId} MacAddress: {machine.MacAddress} IpAddress: {machine.IpAddress}");
                if (client.admin != null)
                {
                    Logger.Warning("Usuário ja está conectado!");
                    client.SendPacket(new API_LOGIN_ADMIN_RESULT_ACK(null, 3));
                    client.Close();
                    return;
                }
                Account accountAdmin = AccountManager.SearchAccountDBAdmin(login);
                if (accountAdmin != null)
                {
                    if (!accountAdmin.ComparePassword(password))
                    {
                        Logger.Warning("Senha incorreta: " + password);
                        client.SendPacket(new API_LOGIN_ADMIN_RESULT_ACK(null, 6));
                        client.Close();
                        return;
                    }
                    if (accountAdmin.access >= AccessLevelEnum.Admin)
                    {
                        client.admin = accountAdmin;
                        client.SendPacket(new API_LOGIN_ADMIN_RESULT_ACK(accountAdmin, 1));
                        client.SendPacket(new API_SERVER_INFO_ACK());
                    }
                    else
                    {
                        client.SendPacket(new API_LOGIN_ADMIN_RESULT_ACK(null, 2));
                        client.Close();
                    }
                }
                else
                {
                    Logger.Warning("Account is null! login: " + login);
                    client.SendPacket(new API_LOGIN_ADMIN_RESULT_ACK(null, 0));
                    client.Close();
                }
            }
            else
            {
                client.SendPacket(new API_LOGIN_ADMIN_RESULT_ACK(null, 5));
                client.Close();
            }
        }
    }
}