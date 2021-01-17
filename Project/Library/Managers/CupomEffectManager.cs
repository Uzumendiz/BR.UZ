using System.Collections.Generic;
using System.IO;
using System.Xml;

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
    public class CupomFlag
    {
        public int ItemId;
        public CupomEffects EffectFlag;
    }

    public static class CupomEffectManager
    {
        private static readonly List<CupomFlag> Effects = new List<CupomFlag>();
        private static readonly string path = "Data/Cupons/CupomFlags.xml";
        public static void Load()
        {
            if (!File.Exists(path))
            {
                Logger.Warning($" [CupomEffectManager] {path} no exists.");
                return;
            }
            GenerateList();
            Logger.Informations($" [CupomEffectManager] Loaded {Effects.Count} effects.");
        }
        private static void GenerateList()
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(path);
                for (XmlNode xmlNode1 = document.FirstChild; xmlNode1 != null; xmlNode1 = xmlNode1.NextSibling)
                {
                    if ("list".Equals(xmlNode1.Name))
                    {
                        for (XmlNode xmlNode2 = xmlNode1.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
                        {
                            if ("cupom".Equals(xmlNode2.Name))
                            {
                                XmlNamedNodeMap xml = xmlNode2.Attributes;
                                CupomFlag cupom = new CupomFlag
                                {
                                    ItemId = int.Parse(xml.GetNamedItem("item_id").Value),
                                    EffectFlag = (CupomEffects)int.Parse(xml.GetNamedItem("effect_flag").Value)
                                };
                                Effects.Add(cupom);
                            }
                        }
                    }
                }
            }
            catch (XmlException ex)
            {
                Logger.Exception(ex);
            }
        }
        public static CupomFlag GetCupomEffect(int id)
        {
            for (int i = 0; i < Effects.Count; i++)
            {
                CupomFlag flag = Effects[i];
                if (flag.ItemId == id)
                {
                    return flag;
                }
            }
            return null;
        }
    }
}