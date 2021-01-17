using System;
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
    public class CharaModel
    {
        public int Id, Life, Type;
    }
    public class CharaXML
    {
        public static List<CharaModel> charas = new List<CharaModel>();
        private static readonly string path = "Data/Battle/Characters.xml";
        public static int GetLifeById(int charaId, int type)
        {
            for (int i = 0; i < charas.Count; i++)
            {
                CharaModel chara = charas[i];
                if (chara.Id == charaId && chara.Type == type)
                {
                    return chara.Life;
                }
            }
            return 100;
        }

        public static void Load()
        {
            if (!File.Exists(path))
            {
                Logger.Warning($" [CharaXML] {path} no exists.");
                return;
            }
            GenerateList();
            Logger.Informations($" [CharaXML] Loaded {charas.Count} characters.");
        }
        public static void ReGenerateList()
        {
            charas.Clear();
            Load();
        }
        private static void GenerateList()
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(path);
                for (XmlNode primaryNode = document.FirstChild; primaryNode != null; primaryNode = primaryNode.NextSibling)
                {
                    if ("list".Equals(primaryNode.Name))
                    {
                        for (XmlNode secundaryNode = primaryNode.FirstChild; secundaryNode != null; secundaryNode = secundaryNode.NextSibling)
                        {
                            if ("Chara".Equals(secundaryNode.Name))
                            {
                                XmlNamedNodeMap xml = secundaryNode.Attributes;
                                CharaModel chara = new CharaModel
                                {
                                    Id = int.Parse(xml.GetNamedItem("Id").Value),
                                    Type = int.Parse(xml.GetNamedItem("Type").Value),
                                    Life = int.Parse(xml.GetNamedItem("Life").Value)
                                };
                                charas.Add(chara);
                            }
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