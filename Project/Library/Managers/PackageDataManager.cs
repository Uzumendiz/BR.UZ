using PointBlank.Auth;
using PointBlank.Game;
using System.Collections.Generic;

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
    public class PackageDataManager
    {
        public static byte[] SLOTINFO;
        /* BATTLE SERVER PACKETS BUFFER */
        public static byte[] Packet66CreatorBuffer;

        /* AUTH SERVER PACKETS BUFFER */
        public static byte[] BASE_COPYRIGTH_ACK;
        public static byte[] SERVER_MESSAGE_EVENT_QUEST_PAK;
        public static byte[] SERVER_MESSAGE_EVENT_RANKUP_PAK;
        public static byte[] A_2678_PAK;
        public static byte[] AUTH_ACCOUNT_KICK_TYPE1_PAK;
        public static byte[] SERVER_MESSAGE_DISCONNECT_2147483904_PAK;
        public static byte[] SERVER_MESSAGE_ERROR_2147487744_PAK;
        public static byte[] AUTH_SERVER_MESSAGE_ITEM_RECEIVE_PAK;
        public static byte[] BASE_LOGIN_0x80000127_PAK;
        public static byte[] BASE_LOGIN_0x80000107_PAK;
        public static byte[] BASE_LOGIN_0x80000121_PAK;
        public static byte[] BASE_LOGIN_0x80000102_PAK;
        public static byte[] BASE_LOGIN_0x80000101_PAK;
        public static byte[] BASE_LOGIN_0x80000105_PAK;
        public static byte[] BASE_LOGIN_0x80000104_PAK;
        public static byte[] PROTOCOL_BASE_USER_INFO_ERROR_ACK;

        /* GAME SERVER PACKETS BUFFER */
        public static byte[] EVENT_VISIT_CONFIRM_0x80001504_PAK;
        public static byte[] EVENT_VISIT_CONFIRM_0x80001500_PAK;
        public static byte[] EVENT_VISIT_CONFIRM_0x80001503_PAK;
        public static byte[] EVENT_VISIT_CONFIRM_0x80001505_PAK;
        public static byte[] EVENT_VISIT_CONFIRM_0x80001502_PAK;

        public static byte[] GAME_ACCOUNT_KICK_TYPE1_PAK;
        public static byte[] SERVER_MESSAGE_ERROR_0x80001000_PAK;
        public static byte[] AUTH_CHECK_NICKNAME_ERROR_PAK;
        public static byte[] AUTH_CHECK_NICKNAME_SUCCESS_PAK;
        public static byte[] AUTH_FIND_USER_2147489795_PAK;
        public static byte[] AUTH_FIND_USER_2147489796_PAK;
        public static byte[] BASE_PROFILE_ENTER_PAK;
        public static byte[] BASE_PROFILE_LEAVE_PAK;
        public static byte[] BASE_SERVER_CHANGE_GAME_PAK;
        public static byte[] BASE_SERVER_PASSW_SUCCESS_PAK;
        public static byte[] BASE_SERVER_PASSW_ERROR_PAK;
        public static byte[] BASE_TITLE_DETACH_ERROR_PAK;
        public static byte[] BASE_TITLE_DETACH_SUCCESS_PAK;
        public static byte[] BASE_TITLE_GET_ERROR_PAK;
        public static byte[] BASE_TITLE_USE_ERROR_PAK;
        public static byte[] BASE_TITLE_USE_SUCCESS_PAK;
        public static byte[] BASE_USER_ENTER_ERROR_PAK;
        public static byte[] BASE_USER_ENTER_SUCCESS_PAK;
        public static byte[] BASE_EXIT_GAME_PAK;
        public static byte[] A_3329_PAK;
        public static byte[] BATTLE_COUNTDOWN_START_PAK;
        public static byte[] BATTLE_COUNTDOWN_STOPBYHOST_PAK;
        public static byte[] BATTLE_READY_ERROR_0x800010AB_PAK;
        public static byte[] BATTLE_READY_ERROR_0x80001009_PAK;
        public static byte[] BATTLE_READY_ERROR_0x80001008_PAK;
        public static byte[] BATTLE_READY_ERROR_0x80001071_PAK;
        public static byte[] BATTLE_READY_ERROR_0x80001072_PAK;
        public static byte[] BATTLE_TUTORIAL_ROUND_END_PAK;
        public static byte[] BATTLE_4VS4_ERROR_PAK;
        public static byte[] VOTEKICK_CANCEL_VOTE_PAK;
        public static byte[] VOTEKICK_CHECK_ERROR_0x800010E4_PAK;
        public static byte[] VOTEKICK_CHECK_ERROR_0x800010E0_PAK;
        public static byte[] VOTEKICK_CHECK_ERROR_0x800010E1_PAK;
        public static byte[] VOTEKICK_CHECK_ERROR_0x800010E2_PAK;
        public static byte[] VOTEKICK_CHECK_SUCCESS_PAK;
        public static byte[] VOTEKICK_KICK_WARNING_PAK;
        public static byte[] VOTEKICK_UPDATE_RESULT_ERROR_PAK;
        public static byte[] BOX_MESSAGE_CREATE_ERROR_0x8000107F_PAK;
        public static byte[] BOX_MESSAGE_CREATE_ERROR_0x8000107E_PAK;
        public static byte[] BOX_MESSAGE_CREATE_ERROR_0x80000000_PAK;
        public static byte[] BOX_MESSAGE_CREATE_SUCCESS_PAK;
        public static byte[] BOX_MESSAGE_SEND_ERROR_0x80000000_PAK;
        public static byte[] BOX_MESSAGE_SEND_SUCCESS_PAK;
        public static byte[] PROTOCOL_CLAN_SAVEINFO3_SUCCESS_ACK;
        public static byte[] PROTOCOL_CLAN_SAVEINFO3_ERROR_ACK;
        public static byte[] PROTOCOL_CLAN_LIST_ENLISTMENTS_ERROR_ACK;
        public static byte[] PROTOCOL_CLAN_REQUEST_INFO_ERROR_ACK;
        public static byte[] PROTOCOL_CLAN_GET_CONTEXT_ENLISTMENTS_ERROR_ACK;
        public static byte[] PROTOCOL_CLAN_REPLACE_NOTICE_2147487835_ACK;
        public static byte[] PROTOCOL_CLAN_REPLACE_NOTICE_2147487859_ACK;
        public static byte[] PROTOCOL_CLAN_REPLACE_NOTICE_SUCCESS_ACK;
        public static byte[] PROTOCOL_CLAN_REPLACE_INTRO_SUCCESS_ACK;
        public static byte[] PROTOCOL_CLAN_REPLACE_INTRO_2147487860_ACK;
        public static byte[] PROTOCOL_CLAN_REPLACE_INTRO_2147487835_ACK;
        public static byte[] PROTOCOL_CLAN_WAR_ACCEPTED_BATTLE_SUCCESS_PAK;
        public static byte[] PROTOCOL_CLAN_WAR_ACCEPTED_BATTLE_ERROR_0x80001092_PAK;
        public static byte[] PROTOCOL_CLAN_WAR_ACCEPTED_BATTLE_ERROR_0x80001091_PAK;
        public static byte[] PROTOCOL_CLAN_WAR_ACCEPTED_BATTLE_ERROR_0x80001090_PAK;
        public static byte[] PROTOCOL_CLAN_WAR_ACCEPTED_BATTLE_ERROR_0x80001094_PAK;
        public static byte[] CLAN_WAR_LEAVE_TEAM_ERROR_PAK;
        public static byte[] CLAN_WAR_LEAVE_TEAM_SUCCESS_PAK;
        public static byte[] CLAN_WAR_MATCH_PROPOSE_ERROR_PAK;
        public static byte[] CLAN_WAR_RECUSED_BATTLE_ERROR_0x80001093_PAK;
        public static byte[] EVENT_VISIT_REWARD_ERROR_USERFAIL_PAK;
        public static byte[] EVENT_VISIT_REWARD_ERROR_ALREADYCHECK_PAK;
        public static byte[] EVENT_VISIT_REWARD_ERROR_UNKNOWN_PAK;
        public static byte[] EVENT_VISIT_REWARD_ERROR_NOTENOUGH_PAK;
        public static byte[] EVENT_VISIT_REWARD_ERROR_WRONGVERSION_PAK;
        public static byte[] EVENT_VISIT_REWARD_SUCCESS_PAK;
        public static byte[] FRIEND_ACCEPT_ERROR_PAK;
        public static byte[] FRIEND_ACCEPT_SUCCESS_PAK;
        public static byte[] FRIEND_INVITE_FOR_ROOM_ERROR_0x80003002_PAK;
        public static byte[] FRIEND_INVITE_FOR_ROOM_ERROR_0x80003003_PAK;
        public static byte[] FRIEND_INVITE_FOR_ROOM_ERROR_0x8000103E_PAK;
        public static byte[] FRIEND_INVITE_FOR_ROOM_ERROR_0x8000103F_PAK;
        public static byte[] FRIEND_INVITE_FOR_ROOM_ERROR_0x8000103D_PAK;
        public static byte[] FRIEND_INVITE_ERROR_0x80001037_PAK;
        public static byte[] FRIEND_INVITE_ERROR_0x80001038_PAK;
        public static byte[] FRIEND_INVITE_ERROR_0x80001039_PAK;
        public static byte[] FRIEND_INVITE_ERROR_0x80001041_PAK;
        public static byte[] FRIEND_INVITE_ERROR_0x80001042_PAK;
        public static byte[] FRIEND_REMOVE_ERROR_PAK;
        public static byte[] FRIEND_REMOVE_SUCCESS_PAK;
        public static byte[] INVENTORY_LEAVE_PAK;
        public static byte[] LOBBY_CREATE_NICKNAME_ERROR_0x80001013_PAK;
        public static byte[] LOBBY_CREATE_NICKNAME_ERROR_0x80000113_PAK;
        public static byte[] LOBBY_CREATE_NICKNAME_SUCCESS_PAK;
        public static byte[] LOBBY_ENTER_PAK;
        public static byte[] LOBBY_LEAVE_SUCCESS_PAK;
        public static byte[] LOBBY_LEAVE_ERROR_PAK;
        public static byte[] LOBBY_QUICKJOIN_ROOM_PAK;
        public static byte[] ROOM_CHANGE_HOST_ERROR_PAK;
        public static byte[] ROOM_CLOSE_SLOT_SUCCESS_PAK;
        public static byte[] ROOM_CLOSE_SLOT_ERROR_PAK;
        public static byte[] GAME_SERVER_MESSAGE_ITEM_RECEIVE_PAK;
        public static byte[] SERVER_MESSAGE_KICK_PLAYER_PAK;
        public static byte[] ROOM_GET_PLAYERINFO_NULL_PAK;
        public static byte[] ROOM_INVITE_RETURN_SUCCESS_PAK;
        public static byte[] ROOM_INVITE_RETURN_ERROR_PAK;
        public static byte[] ROOM_NEW_HOST_ERROR_PAK;
        public static byte[] ROOM_RANDOM_HOST_ERROR_PAK;
        public static byte[] ROOM_GET_HOST_ERROR_PAK;
        public static byte[] SHOP_BUY_2147487767_PAK;
        public static byte[] SHOP_BUY_2147487929_PAK;
        public static byte[] SHOP_BUY_2147487768_PAK;
        public static byte[] SHOP_BUY_2147487769_PAK;
        public static byte[] SHOP_GET_REPAIR_PAK;
        public static byte[] SHOP_TEST2_PAK;
        public static byte[] SHOP_LEAVE_PAK;
        public static byte[] SERVER_MESSAGE_ANNOUNCE_BLOCK_RULE_PAK;
        public static byte[] PROTOCOL_SERVER_MESSAGE_KICK_BATTLE_PLAYER_0x8000100B_ACK;
        public static byte[] PROTOCOL_SERVER_MESSAGE_KICK_BATTLE_PLAYER_0x80001008_ACK;
        public static byte[] PROTOCOL_SERVER_MESSAGE_KICK_BATTLE_PLAYER_0x8000100A_ACK;
        public static byte[] PROTOCOL_BATTLE_STARTBATTLE_ACK;
        public static byte[] LOBBY_CREATE_ROOM_0x80000000_PAK;
        public static byte[] LOBBY_CREATE_ROOM_0x8000107D_PAK;
        public static byte[] LOBBY_GET_PLAYERINFO_NULL_PAK;
        public static byte[] LOBBY_GET_PLAYERINFO2_NULL_PAK;
        public static byte[] LOBBY_JOIN_ROOM_0x8000107C_PAK;
        public static byte[] LOBBY_JOIN_ROOM_0x80001005_PAK;
        public static byte[] LOBBY_JOIN_ROOM_0x80001013_PAK;
        public static byte[] LOBBY_JOIN_ROOM_0x8000100C_PAK;
        public static byte[] LOBBY_JOIN_ROOM_0x80001003_PAK;
        public static byte[] LOBBY_JOIN_ROOM_0x80001004_PAK;

        public static byte[] INVENTORY_ITEM_EQUIP_0x80000000_PAK;
        public static byte[] INVENTORY_ITEM_EQUIP_2147483923_PAK;
        public static byte[] CLAN_CHECK_CREATE_INVITE_SUCCESS_PAK;
        public static byte[] CLAN_CHECK_CREATE_INVITE_0x80000000_PAK;
        public static byte[] CLAN_CHECK_CREATE_INVITE_2147487867_PAK;
        public static byte[] CLAN_CHECK_CREATE_INVITE_0x8000107A_ACK;
        public static byte[] CLAN_CHECK_LOGO_SUCCESS_ACK;
        public static byte[] CLAN_CHECK_LOGO_ERROR_ACK;
        public static byte[] CLAN_CHECK_NAME_SUCCESS_ACK;
        public static byte[] CLAN_CHECK_NAME_ERROR_ACK;
        public static byte[] CLAN_CLIENT_LEAVE_PAK;
        public static byte[] BASE_GET_USER_STATS_NULL_PAK;
        public static byte[] BASE_QUEST_BUY_CARD_SET_0x8000104C_PAK;
        public static byte[] BASE_QUEST_BUY_CARD_SET_0x8000104D_PAK;
        public static byte[] BASE_QUEST_BUY_CARD_SET_0x8000104E_PAK;
        public static byte[] BASE_QUEST_DELETE_CARD_SET_ERROR_PAK;
        public static byte[] BASE_CHANNEL_ENTER_0x80000201_PAK;
        public static byte[] BASE_CHANNEL_ENTER_0x80000202_PAK;
        public static byte[] BASE_CHANNEL_ENTER_0x80000000_PAK;
        public static byte[] BOX_MESSAGE_DELETE_ERROR_PAK;
        public static byte[] BOX_MESSAGE_GIFT_TAKE_0x80000000_PAK;
        public static byte[] INVENTORY_ITEM_EQUIP_2147487785_PAK;
        public static byte[] INVENTORY_ITEM_EXCLUDE_0x80000000_PAK;
        public static byte[] CLAN_CLOSE_2147487850_PAK;
        public static byte[] CLAN_CLOSE_SUCCESS_PAK;
        public static byte[] CLAN_CREATE_INVITE_2147487836_PAK;
        public static byte[] CLAN_CREATE_INVITE_0x80000000_PAK;
        public static byte[] CLAN_CREATE_INVITE_2147487831_PAK;
        public static byte[] CLAN_CREATE_INVITE_2147487848_PAK;
        public static byte[] CLAN_CREATE_0x8000105C_PAK;
        public static byte[] CLAN_CREATE_0x8000104A_PAK;
        public static byte[] CLAN_CREATE_0x8000105A_PAK;
        public static byte[] CLAN_CREATE_0x80001055_PAK;
        public static byte[] CLAN_CREATE_0x8000104B_PAK;
        public static byte[] CLAN_CREATE_0x80001048_PAK;
        public static byte[] CLAN_CREATE_REQUIREMENTS_PAK;
        public static byte[] CLAN_PRIVILEGES_KICK_PAK;
        public static byte[] CLAN_DEPORTATION_2147487833_PAK;
        public static byte[] CLAN_DEPORTATION_SUCCESS_PAK;
        public static byte[] CLAN_PRIVILEGES_DEMOTE_PAK;
        public static byte[] CLAN_COMMISSION_REGULAR_2147487833_PAK;
        public static byte[] CLAN_MEMBER_CONTEXT_ERROR_PAK;
        public static byte[] CLAN_MEMBER_LIST_ERROR_PAK;
        public static byte[] CLAN_MESSAGE_INVITE_0x80000000_PAK;
        public static byte[] CLAN_MESSAGE_INVITE_SUCCESS_PAK;

        public static byte[] CLAN_MESSAGE_REQUEST_ACCEPT_2147487835_PAK;
        public static byte[] CLAN_MESSAGE_REQUEST_ACCEPT_2147487832_PAK;
        public static byte[] CLAN_MESSAGE_REQUEST_ACCEPT_2147487830_PAK;
        public static byte[] BOX_MESSAGE_SEND_PAK;
        public static byte[] CLAN_PLAYER_CLEAN_INVITES_SUCCESS_PAK;
        public static byte[] CLAN_PLAYER_CLEAN_INVITES_2147487835_PAK;
        public static byte[] CLAN_MEMBER_LEAVE_0x8000106B_ACK;
        public static byte[] CLAN_MEMBER_LEAVE_2147487838_ACK;
        public static byte[] CLAN_MEMBER_LEAVE_2147487835_ACK;
        public static byte[] CLAN_MEMBER_LEAVE_SUCCESS_ACK;
        public static byte[] CLAN_PRIVILEGES_AUX_PAK;
        public static byte[] CLAN_COMMISSION_STAFF_2147487833_PAK;
        public static byte[] CLAN_PRIVILEGES_MASTER_PAK;

        public static void Load()
        {
            using (BattlePacketWriter send = new BattlePacketWriter())
            {
                send.WriteB(new byte[10]);
                send.WriteD(4294967295);
                send.WriteB(new byte[28]);
                SLOTINFO = send.memory.ToArray();
            }
            using (PROTOCOL_BASE_COPYRIGTH_ACK packet = new PROTOCOL_BASE_COPYRIGTH_ACK())
            {
                BASE_COPYRIGTH_ACK = packet.GetCompleteBytes(null);
            }
            /* BATTLE SERVER PACKETS BUFFER */
            using (BattlePacketWriter send = new BattlePacketWriter())
            {
                send.WriteC(66);
                send.WriteC(0);
                send.WriteT(0);
                send.WriteC(0);
                send.WriteH(13);
                send.WriteD(0);
                Packet66CreatorBuffer = send.memory.ToArray();
            }
            /* AUTH SERVER PACKETS BUFFER */
            using (PROTOCOL_SERVER_MESSAGE_EVENT_QUEST_ACK packet = new PROTOCOL_SERVER_MESSAGE_EVENT_QUEST_ACK())
            {
                SERVER_MESSAGE_EVENT_QUEST_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_SERVER_MESSAGE_EVENT_RANKUP_ACK packet = new PROTOCOL_SERVER_MESSAGE_EVENT_RANKUP_ACK())
            {
                SERVER_MESSAGE_EVENT_RANKUP_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_BASE_SOURCE_ACK packet = new PROTOCOL_BASE_SOURCE_ACK())
            {
                A_2678_PAK = packet.GetCompleteBytes(null);
            }
            using (Auth.PROTOCOL_BASE_ACCOUNT_KICK_ACK packet = new Auth.PROTOCOL_BASE_ACCOUNT_KICK_ACK(1))
            {
                AUTH_ACCOUNT_KICK_TYPE1_PAK = packet.GetCompleteBytes(null);
            }
            using (Auth.PROTOCOL_SERVER_MESSAGE_DISCONNECT_ACK packet = new Auth.PROTOCOL_SERVER_MESSAGE_DISCONNECT_ACK(2147483904, false))
            {
                SERVER_MESSAGE_DISCONNECT_2147483904_PAK = packet.GetCompleteBytes(null);
            }
            using (Auth.PROTOCOL_SERVER_MESSAGE_ERROR_ACK packet = new Auth.PROTOCOL_SERVER_MESSAGE_ERROR_ACK(2147487744))
            {
                SERVER_MESSAGE_ERROR_2147487744_PAK = packet.GetCompleteBytes(null);
            }
            using (Auth.PROTOCOL_SERVER_MESSAGE_ITEM_RECEIVE_ACK packet = new Auth.PROTOCOL_SERVER_MESSAGE_ITEM_RECEIVE_ACK())
            {
                AUTH_SERVER_MESSAGE_ITEM_RECEIVE_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_BASE_LOGIN_ACK packet = new PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum.Login_INVALID_ACCOUNT, "NULL", 0))
            {
                BASE_LOGIN_0x80000127_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_BASE_LOGIN_ACK packet = new PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum.Login_BLOCK_ACCOUNT, "NULL", 0))
            {
                BASE_LOGIN_0x80000107_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_BASE_LOGIN_ACK packet = new PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum.Login_BLOCK_IP, "NULL", 0))
            {
                BASE_LOGIN_0x80000121_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_BASE_LOGIN_ACK packet = new PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum.Login_USER_PASS_FAIL, "NULL", 0))
            {
                BASE_LOGIN_0x80000102_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_BASE_LOGIN_ACK packet = new PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum.Login_ALREADY_LOGIN_WEB, "NULL", 0))
            {
                BASE_LOGIN_0x80000101_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_BASE_LOGIN_ACK packet = new PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum.Login_TIME_OUT_1, "NULL", 0))
            {
                BASE_LOGIN_0x80000105_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_BASE_LOGIN_ACK packet = new PROTOCOL_BASE_LOGIN_ACK(EventErrorEnum.Login_LOGOUTING, "NULL", 0))
            {
                BASE_LOGIN_0x80000104_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_BASE_USER_INFO_ACK packet = new PROTOCOL_BASE_USER_INFO_ACK(null, null, 0, 0x80000000))
            {
                PROTOCOL_BASE_USER_INFO_ERROR_ACK = packet.GetCompleteBytes(null);
            }
            /* GAME SERVER PACKETS BUFFER */
            using (Game.AUTH_ACCOUNT_KICK_PAK packet = new Game.AUTH_ACCOUNT_KICK_PAK(1))
            {
                GAME_ACCOUNT_KICK_TYPE1_PAK = packet.GetCompleteBytes(null);
            }
            using (Game.SERVER_MESSAGE_ERROR_PAK packet = new Game.SERVER_MESSAGE_ERROR_PAK(0x80001000))
            {
                SERVER_MESSAGE_ERROR_0x80001000_PAK = packet.GetCompleteBytes(null);
            }
            using (AUTH_CHECK_NICKNAME_PAK packet = new AUTH_CHECK_NICKNAME_PAK(2147483923))
            {
                AUTH_CHECK_NICKNAME_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (AUTH_CHECK_NICKNAME_PAK packet = new AUTH_CHECK_NICKNAME_PAK(0))
            {
                AUTH_CHECK_NICKNAME_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (AUTH_FIND_USER_PAK packet = new AUTH_FIND_USER_PAK(null, 2147489795))
            {
                AUTH_FIND_USER_2147489795_PAK = packet.GetCompleteBytes(null);
            }
            using (AUTH_FIND_USER_PAK packet = new AUTH_FIND_USER_PAK(null, 2147489796))
            {
                AUTH_FIND_USER_2147489796_PAK = packet.GetCompleteBytes(null);
            }
            using (BASE_PROFILE_ENTER_PAK packet = new BASE_PROFILE_ENTER_PAK())
            {
                BASE_PROFILE_ENTER_PAK = packet.GetCompleteBytes(null);
            }
            using (BASE_PROFILE_LEAVE_PAK packet = new BASE_PROFILE_LEAVE_PAK())
            {
                BASE_PROFILE_LEAVE_PAK = packet.GetCompleteBytes(null);
            }
            using (Game.BASE_SERVER_CHANGE_PAK packet = new Game.BASE_SERVER_CHANGE_PAK())
            {
                BASE_SERVER_CHANGE_GAME_PAK = packet.GetCompleteBytes(null);
            }
            using (BASE_SERVER_PASSWORD_PAK packet = new BASE_SERVER_PASSWORD_PAK(0))
            {
                BASE_SERVER_PASSW_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (BASE_SERVER_PASSWORD_PAK packet = new BASE_SERVER_PASSWORD_PAK(0x80000000))
            {
                BASE_SERVER_PASSW_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (BASE_TITLE_DETACH_PAK packet = new BASE_TITLE_DETACH_PAK(0x80000000))
            {
                BASE_TITLE_DETACH_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (BASE_TITLE_DETACH_PAK packet = new BASE_TITLE_DETACH_PAK(0))
            {
                BASE_TITLE_DETACH_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (BASE_TITLE_GET_PAK packet = new BASE_TITLE_GET_PAK(0x80001083, -1))
            {
                BASE_TITLE_GET_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (BASE_TITLE_USE_PAK packet = new BASE_TITLE_USE_PAK(0x80000000))
            {
                BASE_TITLE_USE_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (BASE_TITLE_USE_PAK packet = new BASE_TITLE_USE_PAK(0))
            {
                BASE_TITLE_USE_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (BASE_USER_ENTER_PAK packet = new BASE_USER_ENTER_PAK(0x80000000))
            {
                BASE_USER_ENTER_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (BASE_USER_ENTER_PAK packet = new BASE_USER_ENTER_PAK(0))
            {
                BASE_USER_ENTER_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (Game.BASE_USER_EXIT_PAK packet = new Game.BASE_USER_EXIT_PAK())
            {
                BASE_EXIT_GAME_PAK = packet.GetCompleteBytes(null);
            }
            using (A_3329_PAK packet = new A_3329_PAK())
            {
                A_3329_PAK = packet.GetCompleteBytes(null);
            }
            using (BATTLE_COUNTDOWN_PAK packet = new BATTLE_COUNTDOWN_PAK(CountDownEnum.Start))
            {
                BATTLE_COUNTDOWN_START_PAK = packet.GetCompleteBytes(null);
            }
            using (BATTLE_COUNTDOWN_PAK packet = new BATTLE_COUNTDOWN_PAK(CountDownEnum.StopByHost))
            {
                BATTLE_COUNTDOWN_STOPBYHOST_PAK = packet.GetCompleteBytes(null);
            }
            using (BATTLE_READY_ERROR_PAK packet = new BATTLE_READY_ERROR_PAK(0x800010AB))
            {
                BATTLE_READY_ERROR_0x800010AB_PAK = packet.GetCompleteBytes(null);
            }
            using (BATTLE_READY_ERROR_PAK packet = new BATTLE_READY_ERROR_PAK(0x80001009))
            {
                BATTLE_READY_ERROR_0x80001009_PAK = packet.GetCompleteBytes(null);
            }
            using (BATTLE_READY_ERROR_PAK packet = new BATTLE_READY_ERROR_PAK(0x80001008))
            {
                BATTLE_READY_ERROR_0x80001008_PAK = packet.GetCompleteBytes(null);
            }
            using (BATTLE_READY_ERROR_PAK packet = new BATTLE_READY_ERROR_PAK(0x80001071))
            {
                BATTLE_READY_ERROR_0x80001071_PAK = packet.GetCompleteBytes(null);
            }
            using (BATTLE_READY_ERROR_PAK packet = new BATTLE_READY_ERROR_PAK(0x80001072))
            {
                BATTLE_READY_ERROR_0x80001072_PAK = packet.GetCompleteBytes(null);
            }
            using (BATTLE_TUTORIAL_ROUND_END_PAK packet = new BATTLE_TUTORIAL_ROUND_END_PAK())
            {
                BATTLE_TUTORIAL_ROUND_END_PAK = packet.GetCompleteBytes(null);
            }
            using (BATTLE_4VS4_ERROR_PAK packet = new BATTLE_4VS4_ERROR_PAK())
            {
                BATTLE_4VS4_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (VOTEKICK_CANCEL_VOTE_PAK packet = new VOTEKICK_CANCEL_VOTE_PAK())
            {
                VOTEKICK_CANCEL_VOTE_PAK = packet.GetCompleteBytes(null);
            }
            using (VOTEKICK_CHECK_PAK packet = new VOTEKICK_CHECK_PAK(0x800010E4))
            {
                VOTEKICK_CHECK_ERROR_0x800010E4_PAK = packet.GetCompleteBytes(null);
            }
            using (VOTEKICK_CHECK_PAK packet = new VOTEKICK_CHECK_PAK(0x800010E0))
            {
                VOTEKICK_CHECK_ERROR_0x800010E0_PAK = packet.GetCompleteBytes(null);
            }
            using (VOTEKICK_CHECK_PAK packet = new VOTEKICK_CHECK_PAK(0x800010E1))
            {
                VOTEKICK_CHECK_ERROR_0x800010E1_PAK = packet.GetCompleteBytes(null);
            }
            using (VOTEKICK_CHECK_PAK packet = new VOTEKICK_CHECK_PAK(0x800010E2))
            {
                VOTEKICK_CHECK_ERROR_0x800010E2_PAK = packet.GetCompleteBytes(null);
            }
            using (VOTEKICK_CHECK_PAK packet = new VOTEKICK_CHECK_PAK(0))
            {
                VOTEKICK_CHECK_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (VOTEKICK_KICK_WARNING_PAK packet = new VOTEKICK_KICK_WARNING_PAK())
            {
                VOTEKICK_KICK_WARNING_PAK = packet.GetCompleteBytes(null);
            }
            using (VOTEKICK_UPDATE_RESULT_PAK packet = new VOTEKICK_UPDATE_RESULT_PAK(0x800010F1))
            {
                VOTEKICK_UPDATE_RESULT_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (BOX_MESSAGE_CREATE_PAK packet = new BOX_MESSAGE_CREATE_PAK(0x8000107F))
            {
                BOX_MESSAGE_CREATE_ERROR_0x8000107F_PAK = packet.GetCompleteBytes(null);
            }
            using (BOX_MESSAGE_CREATE_PAK packet = new BOX_MESSAGE_CREATE_PAK(0x8000107E))
            {
                BOX_MESSAGE_CREATE_ERROR_0x8000107E_PAK = packet.GetCompleteBytes(null);
            }
            using (BOX_MESSAGE_CREATE_PAK packet = new BOX_MESSAGE_CREATE_PAK(0x80000000))
            {
                BOX_MESSAGE_CREATE_ERROR_0x80000000_PAK = packet.GetCompleteBytes(null);
            }
            using (BOX_MESSAGE_CREATE_PAK packet = new BOX_MESSAGE_CREATE_PAK(0))
            {
                BOX_MESSAGE_CREATE_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (BOX_MESSAGE_SEND_PAK packet = new BOX_MESSAGE_SEND_PAK(0x80000000))
            {
                BOX_MESSAGE_SEND_ERROR_0x80000000_PAK = packet.GetCompleteBytes(null);
            }
            using (BOX_MESSAGE_SEND_PAK packet = new BOX_MESSAGE_SEND_PAK(0))
            {
                BOX_MESSAGE_SEND_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_CLAN_SAVEINFO3_ACK packet = new PROTOCOL_CLAN_SAVEINFO3_ACK(0))
            {
                PROTOCOL_CLAN_SAVEINFO3_SUCCESS_ACK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_CLAN_SAVEINFO3_ACK packet = new PROTOCOL_CLAN_SAVEINFO3_ACK(0x80000000))
            {
                PROTOCOL_CLAN_SAVEINFO3_ERROR_ACK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_CLAN_LIST_ENLISTMENTS_ACK packet = new PROTOCOL_CLAN_LIST_ENLISTMENTS_ACK(-1))
            {
                PROTOCOL_CLAN_LIST_ENLISTMENTS_ERROR_ACK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_CLAN_REQUEST_INFO_ACK packet = new PROTOCOL_CLAN_REQUEST_INFO_ACK(0x80000000))
            {
                PROTOCOL_CLAN_REQUEST_INFO_ERROR_ACK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_CLAN_CONTEXT_ENLISTMENTS_ACK packet = new PROTOCOL_CLAN_CONTEXT_ENLISTMENTS_ACK(4294967295))
            {
                PROTOCOL_CLAN_GET_CONTEXT_ENLISTMENTS_ERROR_ACK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_CLAN_REPLACE_NOTICE_ACK packet = new PROTOCOL_CLAN_REPLACE_NOTICE_ACK(2147487835))
            {
                PROTOCOL_CLAN_REPLACE_NOTICE_2147487835_ACK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_CLAN_REPLACE_NOTICE_ACK packet = new PROTOCOL_CLAN_REPLACE_NOTICE_ACK(2147487859))
            {
                PROTOCOL_CLAN_REPLACE_NOTICE_2147487859_ACK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_CLAN_REPLACE_NOTICE_ACK packet = new PROTOCOL_CLAN_REPLACE_NOTICE_ACK(0))
            {
                PROTOCOL_CLAN_REPLACE_NOTICE_SUCCESS_ACK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_CLAN_REPLACE_INTRO_ACK packet = new PROTOCOL_CLAN_REPLACE_INTRO_ACK(0))
            {
                PROTOCOL_CLAN_REPLACE_INTRO_SUCCESS_ACK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_CLAN_REPLACE_INTRO_ACK packet = new PROTOCOL_CLAN_REPLACE_INTRO_ACK(2147487860))
            {
                PROTOCOL_CLAN_REPLACE_INTRO_2147487860_ACK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_CLAN_REPLACE_INTRO_ACK packet = new PROTOCOL_CLAN_REPLACE_INTRO_ACK(2147487835))
            {
                PROTOCOL_CLAN_REPLACE_INTRO_2147487835_ACK = packet.GetCompleteBytes(null);
            }
            using (CLAN_WAR_ACCEPTED_BATTLE_PAK packet = new CLAN_WAR_ACCEPTED_BATTLE_PAK(0))
            {
                PROTOCOL_CLAN_WAR_ACCEPTED_BATTLE_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_WAR_ACCEPTED_BATTLE_PAK packet = new CLAN_WAR_ACCEPTED_BATTLE_PAK(0x80001092))
            {
                PROTOCOL_CLAN_WAR_ACCEPTED_BATTLE_ERROR_0x80001092_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_WAR_ACCEPTED_BATTLE_PAK packet = new CLAN_WAR_ACCEPTED_BATTLE_PAK(0x80001091))
            {
                PROTOCOL_CLAN_WAR_ACCEPTED_BATTLE_ERROR_0x80001091_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_WAR_ACCEPTED_BATTLE_PAK packet = new CLAN_WAR_ACCEPTED_BATTLE_PAK(0x80001090))
            {
                PROTOCOL_CLAN_WAR_ACCEPTED_BATTLE_ERROR_0x80001090_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_WAR_ACCEPTED_BATTLE_PAK packet = new CLAN_WAR_ACCEPTED_BATTLE_PAK(0x80001094))
            {
                PROTOCOL_CLAN_WAR_ACCEPTED_BATTLE_ERROR_0x80001094_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_WAR_LEAVE_TEAM_PAK packet = new CLAN_WAR_LEAVE_TEAM_PAK(0x80000000))
            {
                CLAN_WAR_LEAVE_TEAM_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_WAR_LEAVE_TEAM_PAK packet = new CLAN_WAR_LEAVE_TEAM_PAK(0))
            {
                CLAN_WAR_LEAVE_TEAM_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_WAR_MATCH_PROPOSE_PAK packet = new CLAN_WAR_MATCH_PROPOSE_PAK(0x80000000))
            {
                CLAN_WAR_MATCH_PROPOSE_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_WAR_RECUSED_BATTLE_PAK packet = new CLAN_WAR_RECUSED_BATTLE_PAK(0x80001093))
            {
                CLAN_WAR_RECUSED_BATTLE_ERROR_0x80001093_PAK = packet.GetCompleteBytes(null);
            }
            using (EVENT_VISIT_REWARD_PAK packet = new EVENT_VISIT_REWARD_PAK(EventErrorEnum.VisitEvent_UserFail))
            {
                EVENT_VISIT_REWARD_ERROR_USERFAIL_PAK = packet.GetCompleteBytes(null);
            }
            using (EVENT_VISIT_REWARD_PAK packet = new EVENT_VISIT_REWARD_PAK(EventErrorEnum.VisitEvent_AlreadyCheck))
            {
                EVENT_VISIT_REWARD_ERROR_ALREADYCHECK_PAK = packet.GetCompleteBytes(null);
            }
            using (EVENT_VISIT_REWARD_PAK packet = new EVENT_VISIT_REWARD_PAK(EventErrorEnum.VisitEvent_Unknown))
            {
                EVENT_VISIT_REWARD_ERROR_UNKNOWN_PAK = packet.GetCompleteBytes(null);
            }
            using (EVENT_VISIT_REWARD_PAK packet = new EVENT_VISIT_REWARD_PAK(EventErrorEnum.VisitEvent_NotEnough))
            {
                EVENT_VISIT_REWARD_ERROR_NOTENOUGH_PAK = packet.GetCompleteBytes(null);
            }
            using (EVENT_VISIT_REWARD_PAK packet = new EVENT_VISIT_REWARD_PAK(EventErrorEnum.VisitEvent_WrongVersion))
            {
                EVENT_VISIT_REWARD_ERROR_WRONGVERSION_PAK = packet.GetCompleteBytes(null);
            }
            using (EVENT_VISIT_REWARD_PAK packet = new EVENT_VISIT_REWARD_PAK(EventErrorEnum.VisitEvent_Success))
            {
                EVENT_VISIT_REWARD_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (FRIEND_ACCEPT_PAK packet = new FRIEND_ACCEPT_PAK(0x80000000))
            {
                FRIEND_ACCEPT_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (FRIEND_ACCEPT_PAK packet = new FRIEND_ACCEPT_PAK(0))
            {
                FRIEND_ACCEPT_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (FRIEND_INVITE_FOR_ROOM_PAK packet = new FRIEND_INVITE_FOR_ROOM_PAK(0x80003002))
            {
                FRIEND_INVITE_FOR_ROOM_ERROR_0x80003002_PAK = packet.GetCompleteBytes(null);
            }
            using (FRIEND_INVITE_FOR_ROOM_PAK packet = new FRIEND_INVITE_FOR_ROOM_PAK(0x80003003))
            {
                FRIEND_INVITE_FOR_ROOM_ERROR_0x80003003_PAK = packet.GetCompleteBytes(null);
            }
            using (FRIEND_INVITE_FOR_ROOM_PAK packet = new FRIEND_INVITE_FOR_ROOM_PAK(0x8000103E))
            {
                FRIEND_INVITE_FOR_ROOM_ERROR_0x8000103E_PAK = packet.GetCompleteBytes(null);
            }
            using (FRIEND_INVITE_FOR_ROOM_PAK packet = new FRIEND_INVITE_FOR_ROOM_PAK(0x8000103F))
            {
                FRIEND_INVITE_FOR_ROOM_ERROR_0x8000103F_PAK = packet.GetCompleteBytes(null);
            }
            using (FRIEND_INVITE_FOR_ROOM_PAK packet = new FRIEND_INVITE_FOR_ROOM_PAK(0x8000103D))
            {
                FRIEND_INVITE_FOR_ROOM_ERROR_0x8000103D_PAK = packet.GetCompleteBytes(null);
            }
            using (FRIEND_INVITE_PAK packet = new FRIEND_INVITE_PAK(0x80001037))
            {
                FRIEND_INVITE_ERROR_0x80001037_PAK = packet.GetCompleteBytes(null);
            }
            using (FRIEND_INVITE_PAK packet = new FRIEND_INVITE_PAK(0x80001038))
            {
                FRIEND_INVITE_ERROR_0x80001038_PAK = packet.GetCompleteBytes(null);
            }
            using (FRIEND_INVITE_PAK packet = new FRIEND_INVITE_PAK(0x80001039))
            {
                FRIEND_INVITE_ERROR_0x80001039_PAK = packet.GetCompleteBytes(null);
            }
            using (FRIEND_INVITE_PAK packet = new FRIEND_INVITE_PAK(0x80001041))
            {
                FRIEND_INVITE_ERROR_0x80001041_PAK = packet.GetCompleteBytes(null);
            }
            using (FRIEND_INVITE_PAK packet = new FRIEND_INVITE_PAK(0x80001042))
            {
                FRIEND_INVITE_ERROR_0x80001042_PAK = packet.GetCompleteBytes(null);
            }
            using (FRIEND_REMOVE_PAK packet = new FRIEND_REMOVE_PAK(0x80000000))
            {
                FRIEND_REMOVE_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (FRIEND_REMOVE_PAK packet = new FRIEND_REMOVE_PAK(0))
            {
                FRIEND_REMOVE_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (INVENTORY_LEAVE_PAK packet = new INVENTORY_LEAVE_PAK(0))
            {
                INVENTORY_LEAVE_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_LOBBY_CREATE_NICKNAME_ACK packet = new PROTOCOL_LOBBY_CREATE_NICKNAME_ACK(0x80001013))
            {
                LOBBY_CREATE_NICKNAME_ERROR_0x80001013_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_LOBBY_CREATE_NICKNAME_ACK packet = new PROTOCOL_LOBBY_CREATE_NICKNAME_ACK(0x80000113))
            {
                LOBBY_CREATE_NICKNAME_ERROR_0x80000113_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_LOBBY_CREATE_NICKNAME_ACK packet = new PROTOCOL_LOBBY_CREATE_NICKNAME_ACK(0))
            {
                LOBBY_CREATE_NICKNAME_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (LOBBY_ENTER_PAK packet = new LOBBY_ENTER_PAK())
            {
                LOBBY_ENTER_PAK = packet.GetCompleteBytes(null);
            }
            using (LOBBY_LEAVE_PAK packet = new LOBBY_LEAVE_PAK(0))
            {
                LOBBY_LEAVE_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (LOBBY_LEAVE_PAK packet = new LOBBY_LEAVE_PAK(0x80000000))
            {
                LOBBY_LEAVE_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (LOBBY_QUICKJOIN_ROOM_PAK packet = new LOBBY_QUICKJOIN_ROOM_PAK())
            {
                LOBBY_QUICKJOIN_ROOM_PAK = packet.GetCompleteBytes(null);
            }
            using (ROOM_CHANGE_HOST_PAK packet = new ROOM_CHANGE_HOST_PAK(0x80000000))
            {
                ROOM_CHANGE_HOST_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_ROOM_CLOSE_SLOT_ACK packet = new PROTOCOL_ROOM_CLOSE_SLOT_ACK(0))
            {
                ROOM_CLOSE_SLOT_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_ROOM_CLOSE_SLOT_ACK packet = new PROTOCOL_ROOM_CLOSE_SLOT_ACK(2147484673))
            {
                ROOM_CLOSE_SLOT_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (Game.SERVER_MESSAGE_ITEM_RECEIVE_PAK packet = new Game.SERVER_MESSAGE_ITEM_RECEIVE_PAK(0))
            {
                GAME_SERVER_MESSAGE_ITEM_RECEIVE_PAK = packet.GetCompleteBytes(null);
            }
            using (SERVER_MESSAGE_KICK_PLAYER_PAK packet = new SERVER_MESSAGE_KICK_PLAYER_PAK())
            {
                SERVER_MESSAGE_KICK_PLAYER_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_ROOM_GET_PLAYERINFO_ACK packet = new PROTOCOL_ROOM_GET_PLAYERINFO_ACK(null, false))
            {
                ROOM_GET_PLAYERINFO_NULL_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_ROOM_INVITE_RETURN_ACK packet = new PROTOCOL_ROOM_INVITE_RETURN_ACK(0))
            {
                ROOM_INVITE_RETURN_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_ROOM_INVITE_RETURN_ACK packet = new PROTOCOL_ROOM_INVITE_RETURN_ACK(0x80000000))
            {
                ROOM_INVITE_RETURN_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_ROOM_NEW_HOST_ACK packet = new PROTOCOL_ROOM_NEW_HOST_ACK(0x80000000))
            {
                ROOM_NEW_HOST_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_ROOM_RANDOM_HOST_ACK packet = new PROTOCOL_ROOM_RANDOM_HOST_ACK(0x80000000))
            {
                ROOM_RANDOM_HOST_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_ROOM_GET_HOST_ACK packet = new PROTOCOL_ROOM_GET_HOST_ACK(0x80000000))
            {
                ROOM_GET_HOST_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (SHOP_BUY_PAK packet = new SHOP_BUY_PAK(2147487767))
            {
                SHOP_BUY_2147487767_PAK = packet.GetCompleteBytes(null);
            }
            using (SHOP_BUY_PAK packet = new SHOP_BUY_PAK(2147487929))
            {
                SHOP_BUY_2147487929_PAK = packet.GetCompleteBytes(null);
            }
            using (SHOP_BUY_PAK packet = new SHOP_BUY_PAK(2147487768))
            {
                SHOP_BUY_2147487768_PAK = packet.GetCompleteBytes(null);
            }
            using (SHOP_BUY_PAK packet = new SHOP_BUY_PAK(2147487769))
            {
                SHOP_BUY_2147487769_PAK = packet.GetCompleteBytes(null);
            }
            using (SHOP_GET_REPAIR_PAK packet = new SHOP_GET_REPAIR_PAK())
            {
                SHOP_GET_REPAIR_PAK = packet.GetCompleteBytes(null);
            }
            using (SHOP_TEST2_PAK packet = new SHOP_TEST2_PAK())
            {
                SHOP_TEST2_PAK = packet.GetCompleteBytes(null);
            }
            using (SHOP_LEAVE_PAK packet = new SHOP_LEAVE_PAK())
            {
                SHOP_LEAVE_PAK = packet.GetCompleteBytes(null);
            }
            using (Game.SERVER_MESSAGE_ANNOUNCE_PAK packet = new Game.SERVER_MESSAGE_ANNOUNCE_PAK("Não foi possivel alterar para este equipamento devido as regras da sala."))
            {
                SERVER_MESSAGE_ANNOUNCE_BLOCK_RULE_PAK = packet.GetCompleteBytes(null);
            }
            using (SERVER_MESSAGE_KICK_BATTLE_PLAYER_PAK packet = new SERVER_MESSAGE_KICK_BATTLE_PLAYER_PAK(EventErrorEnum.Battle_First_Hole))
            {
                PROTOCOL_SERVER_MESSAGE_KICK_BATTLE_PLAYER_0x8000100B_ACK = packet.GetCompleteBytes(null);
            }
            using (SERVER_MESSAGE_KICK_BATTLE_PLAYER_PAK packet = new SERVER_MESSAGE_KICK_BATTLE_PLAYER_PAK(EventErrorEnum.Battle_No_Real_IP))
            {
                PROTOCOL_SERVER_MESSAGE_KICK_BATTLE_PLAYER_0x80001008_ACK = packet.GetCompleteBytes(null);
            }
            using (SERVER_MESSAGE_KICK_BATTLE_PLAYER_PAK packet = new SERVER_MESSAGE_KICK_BATTLE_PLAYER_PAK(EventErrorEnum.Battle_First_MainLoad))
            {
                PROTOCOL_SERVER_MESSAGE_KICK_BATTLE_PLAYER_0x8000100A_ACK = packet.GetCompleteBytes(null);
            }
            using (BATTLE_STARTBATTLE_PAK packet = new BATTLE_STARTBATTLE_PAK())
            {
                PROTOCOL_BATTLE_STARTBATTLE_ACK = packet.GetCompleteBytes(null);
            }
            using (LOBBY_CREATE_ROOM_PAK packet = new LOBBY_CREATE_ROOM_PAK(0x80000000, null, null))
            {
                LOBBY_CREATE_ROOM_0x80000000_PAK = packet.GetCompleteBytes(null);
            }
            using (LOBBY_CREATE_ROOM_PAK packet = new LOBBY_CREATE_ROOM_PAK(0x8000107D, null, null))
            {
                LOBBY_CREATE_ROOM_0x8000107D_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_LOBBY_PLAYER_STATISTICS_ACK packet = new PROTOCOL_LOBBY_PLAYER_STATISTICS_ACK(null))
            {
                LOBBY_GET_PLAYERINFO_NULL_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_LOBBY_PLAYER_EQUIPMENTS_ACK packet = new PROTOCOL_LOBBY_PLAYER_EQUIPMENTS_ACK(null))
            {
                LOBBY_GET_PLAYERINFO2_NULL_PAK = packet.GetCompleteBytes(null);
            }
            using (LOBBY_JOIN_ROOM_PAK packet = new LOBBY_JOIN_ROOM_PAK(0x8000107C))
            {
                LOBBY_JOIN_ROOM_0x8000107C_PAK = packet.GetCompleteBytes(null);
            }
            using (LOBBY_JOIN_ROOM_PAK packet = new LOBBY_JOIN_ROOM_PAK(0x80001005))
            {
                LOBBY_JOIN_ROOM_0x80001005_PAK = packet.GetCompleteBytes(null);
            }
            using (LOBBY_JOIN_ROOM_PAK packet = new LOBBY_JOIN_ROOM_PAK(0x80001013))
            {
                LOBBY_JOIN_ROOM_0x80001013_PAK = packet.GetCompleteBytes(null);
            }
            using (LOBBY_JOIN_ROOM_PAK packet = new LOBBY_JOIN_ROOM_PAK(0x8000100C))
            {
                LOBBY_JOIN_ROOM_0x8000100C_PAK = packet.GetCompleteBytes(null);
            }
            using (LOBBY_JOIN_ROOM_PAK packet = new LOBBY_JOIN_ROOM_PAK(0x80001003))
            {
                LOBBY_JOIN_ROOM_0x80001003_PAK = packet.GetCompleteBytes(null);
            }
            using (LOBBY_JOIN_ROOM_PAK packet = new LOBBY_JOIN_ROOM_PAK(0x80001004))
            {
                LOBBY_JOIN_ROOM_0x80001004_PAK = packet.GetCompleteBytes(null);
            }
            using (INVENTORY_ITEM_EQUIP_PAK packet = new INVENTORY_ITEM_EQUIP_PAK(0x80000000))
            {
                INVENTORY_ITEM_EQUIP_0x80000000_PAK = packet.GetCompleteBytes(null);
            }
            using (INVENTORY_ITEM_EQUIP_PAK packet = new INVENTORY_ITEM_EQUIP_PAK(2147483923))
            {
                INVENTORY_ITEM_EQUIP_2147483923_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_CHECK_CREATE_INVITE_PAK packet = new CLAN_CHECK_CREATE_INVITE_PAK(0))
            {
                CLAN_CHECK_CREATE_INVITE_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_CHECK_CREATE_INVITE_PAK packet = new CLAN_CHECK_CREATE_INVITE_PAK(0x80000000))
            {
                CLAN_CHECK_CREATE_INVITE_0x80000000_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_CHECK_CREATE_INVITE_PAK packet = new CLAN_CHECK_CREATE_INVITE_PAK(2147487867))
            {
                CLAN_CHECK_CREATE_INVITE_2147487867_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_CHECK_CREATE_INVITE_PAK packet = new CLAN_CHECK_CREATE_INVITE_PAK(0x8000107A))
            {
                CLAN_CHECK_CREATE_INVITE_0x8000107A_ACK = packet.GetCompleteBytes(null);
            }      
            using (CLAN_CHECK_DUPLICATE_MARK_PAK packet = new CLAN_CHECK_DUPLICATE_MARK_PAK(0))
            {
                CLAN_CHECK_LOGO_SUCCESS_ACK = packet.GetCompleteBytes(null);
            }
            using (CLAN_CHECK_DUPLICATE_MARK_PAK packet = new CLAN_CHECK_DUPLICATE_MARK_PAK(0x80000000))
            {
                CLAN_CHECK_LOGO_ERROR_ACK = packet.GetCompleteBytes(null);
            }
            using (CLAN_CHECK_DUPLICATE_NAME_PAK packet = new CLAN_CHECK_DUPLICATE_NAME_PAK(0))
            {
                CLAN_CHECK_NAME_SUCCESS_ACK = packet.GetCompleteBytes(null);
            }
            using (CLAN_CHECK_DUPLICATE_NAME_PAK packet = new CLAN_CHECK_DUPLICATE_NAME_PAK(0x80000000))
            {
                CLAN_CHECK_NAME_ERROR_ACK = packet.GetCompleteBytes(null);
            }
            using (CLAN_CLIENT_LEAVE_PAK packet = new CLAN_CLIENT_LEAVE_PAK())
            {
                CLAN_CLIENT_LEAVE_PAK = packet.GetCompleteBytes(null);
            }
            using (BOX_MESSAGE_DELETE_PAK packet = new BOX_MESSAGE_DELETE_PAK(new List<object>(), 0x80000000))
            {
                BOX_MESSAGE_DELETE_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (BASE_GET_USER_STATS_PAK packet = new BASE_GET_USER_STATS_PAK(null))
            {
                BASE_GET_USER_STATS_NULL_PAK = packet.GetCompleteBytes(null);
            }

            using (BASE_QUEST_BUY_CARD_SET_PAK packet = new BASE_QUEST_BUY_CARD_SET_PAK(0x8000104C, null))
            {
                BASE_QUEST_BUY_CARD_SET_0x8000104C_PAK = packet.GetCompleteBytes(null);
            }
            using (BASE_QUEST_BUY_CARD_SET_PAK packet = new BASE_QUEST_BUY_CARD_SET_PAK(0x8000104D, null))
            {
                BASE_QUEST_BUY_CARD_SET_0x8000104D_PAK = packet.GetCompleteBytes(null);
            }
            using (BASE_QUEST_BUY_CARD_SET_PAK packet = new BASE_QUEST_BUY_CARD_SET_PAK(0x8000104E, null))
            {
                BASE_QUEST_BUY_CARD_SET_0x8000104E_PAK = packet.GetCompleteBytes(null);
            }
            using (BASE_QUEST_DELETE_CARD_SET_PAK packet = new BASE_QUEST_DELETE_CARD_SET_PAK(null, 0x80001050))
            {
                BASE_QUEST_DELETE_CARD_SET_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (BASE_CHANNEL_ENTER_PAK packet = new BASE_CHANNEL_ENTER_PAK(0x80000201))
            {
                BASE_CHANNEL_ENTER_0x80000201_PAK = packet.GetCompleteBytes(null);
            }
            using (BASE_CHANNEL_ENTER_PAK packet = new BASE_CHANNEL_ENTER_PAK(0x80000202))
            {
                BASE_CHANNEL_ENTER_0x80000202_PAK = packet.GetCompleteBytes(null);
            }
            using (BASE_CHANNEL_ENTER_PAK packet = new BASE_CHANNEL_ENTER_PAK(0x80000000))
            {
                BASE_CHANNEL_ENTER_0x80000000_PAK = packet.GetCompleteBytes(null);
            }
            using (BOX_MESSAGE_GIFT_TAKE_PAK packet = new BOX_MESSAGE_GIFT_TAKE_PAK(0x80000000))
            {
                BOX_MESSAGE_GIFT_TAKE_0x80000000_PAK = packet.GetCompleteBytes(null);
            }
            using (INVENTORY_ITEM_EQUIP_PAK packet = new INVENTORY_ITEM_EQUIP_PAK(2147487785))
            {
                INVENTORY_ITEM_EQUIP_2147487785_PAK = packet.GetCompleteBytes(null);
            }
            using (INVENTORY_ITEM_EXCLUDE_PAK packet = new INVENTORY_ITEM_EXCLUDE_PAK(0x80000000))
            {
                INVENTORY_ITEM_EXCLUDE_0x80000000_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_CLOSE_PAK packet = new CLAN_CLOSE_PAK(2147487850))
            {
                CLAN_CLOSE_2147487850_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_CLOSE_PAK packet = new CLAN_CLOSE_PAK(0))
            {
                CLAN_CLOSE_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_CLAN_CREATE_INVITE_ACK packet = new PROTOCOL_CLAN_CREATE_INVITE_ACK(2147487836, 0))
            {
                CLAN_CREATE_INVITE_2147487836_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_CLAN_CREATE_INVITE_ACK packet = new PROTOCOL_CLAN_CREATE_INVITE_ACK(0x80000000, 0))
            {
                CLAN_CREATE_INVITE_0x80000000_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_CLAN_CREATE_INVITE_ACK packet = new PROTOCOL_CLAN_CREATE_INVITE_ACK(2147487831, 0))
            {
                CLAN_CREATE_INVITE_2147487831_PAK = packet.GetCompleteBytes(null);
            }
            using (PROTOCOL_CLAN_CREATE_INVITE_ACK packet = new PROTOCOL_CLAN_CREATE_INVITE_ACK(2147487848, 0))
            {
                CLAN_CREATE_INVITE_2147487848_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_CREATE_PAK packet = new CLAN_CREATE_PAK(0x8000105C, null, null))
            {
                CLAN_CREATE_0x8000105C_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_CREATE_PAK packet = new CLAN_CREATE_PAK(0x8000104A, null, null))
            {
                CLAN_CREATE_0x8000104A_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_CREATE_PAK packet = new CLAN_CREATE_PAK(0x8000105A, null, null))
            {
                CLAN_CREATE_0x8000105A_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_CREATE_PAK packet = new CLAN_CREATE_PAK(0x80001055, null, null))
            {
                CLAN_CREATE_0x80001055_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_CREATE_PAK packet = new CLAN_CREATE_PAK(0x8000104B, null, null))
            {
                CLAN_CREATE_0x8000104B_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_CREATE_PAK packet = new CLAN_CREATE_PAK(0x80001048, null, null))
            {
                CLAN_CREATE_0x80001048_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_CREATE_REQUIREMENTS_PAK packet = new CLAN_CREATE_REQUIREMENTS_PAK())
            {
                CLAN_CREATE_REQUIREMENTS_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_PRIVILEGES_KICK_PAK packet = new CLAN_PRIVILEGES_KICK_PAK())
            {
                CLAN_PRIVILEGES_KICK_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_DEPORTATION_PAK packet = new CLAN_DEPORTATION_PAK(2147487833))
            {
                CLAN_DEPORTATION_2147487833_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_DEPORTATION_PAK packet = new CLAN_DEPORTATION_PAK(1))
            {
                CLAN_DEPORTATION_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_PRIVILEGES_DEMOTE_PAK packet = new CLAN_PRIVILEGES_DEMOTE_PAK())
            {
                CLAN_PRIVILEGES_DEMOTE_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_COMMISSION_REGULAR_PAK packet = new CLAN_COMMISSION_REGULAR_PAK(2147487833))
            {
                CLAN_COMMISSION_REGULAR_2147487833_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_MEMBER_CONTEXT_PAK packet = new CLAN_MEMBER_CONTEXT_PAK(-1))
            {
                CLAN_MEMBER_CONTEXT_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_MEMBER_LIST_PAK packet = new CLAN_MEMBER_LIST_PAK(-1))
            {
                CLAN_MEMBER_LIST_ERROR_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_MESSAGE_INVITE_PAK packet = new CLAN_MESSAGE_INVITE_PAK(0x80000000))
            {
                CLAN_MESSAGE_INVITE_0x80000000_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_MESSAGE_INVITE_PAK packet = new CLAN_MESSAGE_INVITE_PAK(0))
            {
                CLAN_MESSAGE_INVITE_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_MESSAGE_REQUEST_ACCEPT_PAK packet = new CLAN_MESSAGE_REQUEST_ACCEPT_PAK(2147487835))
            {
                CLAN_MESSAGE_REQUEST_ACCEPT_2147487835_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_MESSAGE_REQUEST_ACCEPT_PAK packet = new CLAN_MESSAGE_REQUEST_ACCEPT_PAK(2147487832))
            {
                CLAN_MESSAGE_REQUEST_ACCEPT_2147487832_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_MESSAGE_REQUEST_ACCEPT_PAK packet = new CLAN_MESSAGE_REQUEST_ACCEPT_PAK(2147487830))
            {
                CLAN_MESSAGE_REQUEST_ACCEPT_2147487830_PAK = packet.GetCompleteBytes(null);
            }
            using (BOX_MESSAGE_SEND_PAK packet = new BOX_MESSAGE_SEND_PAK(0))
            {
                BOX_MESSAGE_SEND_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_PLAYER_CLEAN_INVITES_PAK packet = new CLAN_PLAYER_CLEAN_INVITES_PAK(0))
            {
                CLAN_PLAYER_CLEAN_INVITES_SUCCESS_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_PLAYER_CLEAN_INVITES_PAK packet = new CLAN_PLAYER_CLEAN_INVITES_PAK(2147487835))
            {
                CLAN_PLAYER_CLEAN_INVITES_2147487835_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_PLAYER_LEAVE_PAK packet = new CLAN_PLAYER_LEAVE_PAK(0x8000106B))
            {
                CLAN_MEMBER_LEAVE_0x8000106B_ACK = packet.GetCompleteBytes(null);
            }
            using (CLAN_PLAYER_LEAVE_PAK packet = new CLAN_PLAYER_LEAVE_PAK(2147487838))
            {
                CLAN_MEMBER_LEAVE_2147487838_ACK = packet.GetCompleteBytes(null);
            }
            using (CLAN_PLAYER_LEAVE_PAK packet = new CLAN_PLAYER_LEAVE_PAK(2147487835))
            {
                CLAN_MEMBER_LEAVE_2147487835_ACK = packet.GetCompleteBytes(null);
            }
            using (CLAN_PLAYER_LEAVE_PAK packet = new CLAN_PLAYER_LEAVE_PAK(0))
            {
                CLAN_MEMBER_LEAVE_SUCCESS_ACK = packet.GetCompleteBytes(null);
            }
            using (CLAN_PRIVILEGES_AUX_PAK packet = new CLAN_PRIVILEGES_AUX_PAK())
            {
                CLAN_PRIVILEGES_AUX_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_COMMISSION_STAFF_PAK packet = new CLAN_COMMISSION_STAFF_PAK(2147487833))
            {
                CLAN_COMMISSION_STAFF_2147487833_PAK = packet.GetCompleteBytes(null);
            }
            using (CLAN_PRIVILEGES_MASTER_PAK packet = new CLAN_PRIVILEGES_MASTER_PAK())
            {
                CLAN_PRIVILEGES_MASTER_PAK = packet.GetCompleteBytes(null);
            }
            using (EVENT_VISIT_CONFIRM_PAK packet = new EVENT_VISIT_CONFIRM_PAK(EventErrorEnum.VisitEvent_UserFail, null, null))
            {
                EVENT_VISIT_CONFIRM_0x80001500_PAK = packet.GetCompleteBytes(null);
            }
            using (EVENT_VISIT_CONFIRM_PAK packet = new EVENT_VISIT_CONFIRM_PAK(EventErrorEnum.VisitEvent_WrongVersion, null, null))
            {
                EVENT_VISIT_CONFIRM_0x80001503_PAK = packet.GetCompleteBytes(null);
            }
            using (EVENT_VISIT_CONFIRM_PAK packet = new EVENT_VISIT_CONFIRM_PAK(EventErrorEnum.VisitEvent_Unknown, null, null))
            {
                EVENT_VISIT_CONFIRM_0x80001505_PAK = packet.GetCompleteBytes(null);
            }
            using (EVENT_VISIT_CONFIRM_PAK packet = new EVENT_VISIT_CONFIRM_PAK(EventErrorEnum.VisitEvent_AlreadyCheck, null, null))
            {
                EVENT_VISIT_CONFIRM_0x80001502_PAK = packet.GetCompleteBytes(null);
            }
        }
    }
}