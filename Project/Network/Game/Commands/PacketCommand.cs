﻿/* 
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
    public abstract class PacketCommand
    {
        protected Account administrador;
        protected Room room;
        protected string response;
        public abstract void RunImplement();
        public bool Set(Account administrador, Room room)
        {
            this.administrador = administrador;
            this.room = room;
            return true;
        }

        public string GetResponse()
        {
            return response;
        }
    }
}