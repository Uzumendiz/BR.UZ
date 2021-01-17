using NetFwTypeLib;
using System;

/* 
    [PORTUGUESE BRAZIL]
    Este código-fonte é de autoria da <BR.UZ/>, MomzGames, ExploitTeam.

    BR.UZ é um software privado: você não pode usar,
    redistribuir e/ou modificar sem autorização.

    Você deveria ter recebido uma autorização com esse software/codigo-fonte
    para ter cópias, se você não recebeu, encerre/exclua imediatamente 
    todos os vestígios do mesmo para não ter futuros problemas.

    Atenciosamente, <BR.UZ/>, MomzGames, ExploitTeam.
*/

/*
    [INGLISH USA]
    This source code is written by <BR.UZ/>, MomzGames, ExploitTeam.

    BR.UZ a private software: you will not be able to use it,
    redistribute it and / or modify without authorization.

    You should have received an authorization with this software/source code
    to have copies, if you did not receive it, immediately shutdown/exclude
    all traces of it so that you have no future problems.

    Sincerely, <BR.UZ/>, MomzGames, ExploitTeam.
*/

namespace PointBlank
{
    public class FirewallSecurity
    {
        public static string FirewallRuleNameApiTCP, FirewallRuleNameAuthTCP, FirewallRuleNameGameTCP;
        public static string[] FirewallRuleNameBattleUDP;
        public static void LoadInstances(string processName, int sessionsBattle)
        {
            FirewallRuleNameApiTCP = $"{processName} Allow Api UDP Connections";
            FirewallRuleNameAuthTCP = $"{processName} Allow Auth TCP Connections";
            FirewallRuleNameGameTCP = $"{processName} Allow Game TCP Connections";

            FirewallRuleNameBattleUDP = new string[sessionsBattle];
            for (int i = 0; i < sessionsBattle; i++) 
            {
                FirewallRuleNameBattleUDP[i] = $"{processName} Allow Battle UDP Connections " + Settings.PortBattle + i;
            }
        }

        public static void CreateRuleAllow(string ruleName, string ip, int port, NET_FW_IP_PROTOCOL_ protocol)
        {
            RemoveRule(ruleName);
            try
            {
                INetFwRule2 inboundRule = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                inboundRule.Name = ruleName;
                inboundRule.Description = $"Allow inbound traffic from users over port {port}";
                inboundRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                inboundRule.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL;
                inboundRule.Protocol = (int)protocol;
                //inboundRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
                //inboundRule.Grouping = "@firewallapi.dll,-23255";

                inboundRule.RemoteAddresses = ip;
                inboundRule.LocalPorts = port.ToString();
                inboundRule.Enabled = true;


                firewallPolicy.Rules.Add(inboundRule);
                Logger.White($" [Firewall] Allow {(protocol == NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP ? "Tcp" : protocol == NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP ? "Udp" : "Unknown")} connection rule on port {port} has been registered in windows firewall.");
                firewallPolicy = null;
                inboundRule = null;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public static void RemoveRule(string RuleName)
        {
            try
            {
                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                foreach (INetFwRule rule in firewallPolicy.Rules)
                {
                    if (rule.Name.IndexOf(RuleName) != -1)
                    {
                        firewallPolicy.Rules.Remove(rule.Name);
                        break;
                    }
                }
                firewallPolicy = null;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #region UDP
        public static void AddRuleUdp(string address, int sessionPort) //RemoteIP
        {
            try
            {
                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                address += "/255.255.255.255";
                foreach (INetFwRule rule in firewallPolicy.Rules)
                {
                    if (rule.Name.IndexOf(FirewallRuleNameBattleUDP[sessionPort - Settings.PortBattle]) != -1 && !rule.RemoteAddresses.Contains(address))
                    {
                        rule.RemoteAddresses += "," + address;
                        break;
                    }
                }
                firewallPolicy = null;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
        public static void RemoveRuleUdp(string address, int sessionPort) //RemoteIP
        {
            try
            {
                if (sessionPort == 0)
                {
                    return;
                }
                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                address += "/255.255.255.255";
                foreach (INetFwRule rule in firewallPolicy.Rules)
                {
                    if (rule.Name.IndexOf(FirewallRuleNameBattleUDP[sessionPort - Settings.PortBattle]) != -1 && rule.RemoteAddresses.Contains(address))
                    {
                        string IpList = rule.RemoteAddresses;
                        IpList = IpList.Replace(address, "");
                        IpList = IpList.Replace(",,", ",");
                        rule.RemoteAddresses = IpList;
                        break;
                    }
                }
                firewallPolicy = null;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
        #endregion

        #region TCP
        public static void AddRuleTcp(string address) //RemoteIP
        {
            try
            {
                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                address += "/255.255.255.255";
                foreach (INetFwRule rule in firewallPolicy.Rules)
                {
                    if (rule.Name.IndexOf(FirewallRuleNameGameTCP) != -1 && !rule.RemoteAddresses.Contains(address))
                    {
                        rule.RemoteAddresses += "," + address;
                        Application.Counts++;
                        break;
                    }
                }
                firewallPolicy = null;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
        public static void RemoveRuleTcp(string address) //RemoteIP
        {
            try
            {
                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                address += "/255.255.255.255";
                foreach (INetFwRule rule in firewallPolicy.Rules)
                {
                    if (rule.Name.IndexOf(FirewallRuleNameGameTCP) != -1 && rule.RemoteAddresses.Contains(address))
                    {
                        string IpList = rule.RemoteAddresses;
                        IpList = IpList.Replace(address, "");
                        IpList = IpList.Replace(",,", ",");
                        rule.RemoteAddresses = IpList;
                        break;
                    }
                }
                firewallPolicy = null;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
        #endregion
    }
}
