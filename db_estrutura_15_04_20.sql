/*
Navicat PGSQL Data Transfer

Source Server         : localhost
Source Server Version : 90426
Source Host           : localhost:6432
Source Database       : postgres
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90426
File Encoding         : 65001

Date: 2020-04-15 19:36:13
*/


-- ----------------------------
-- Sequence structure for account_id_seq
-- ----------------------------
DROP SEQUENCE "public"."account_id_seq";
CREATE SEQUENCE "public"."account_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 32
 CACHE 1;
SELECT setval('"public"."account_id_seq"', 32, true);

-- ----------------------------
-- Sequence structure for accounts_id_seq
-- ----------------------------
DROP SEQUENCE "public"."accounts_id_seq";
CREATE SEQUENCE "public"."accounts_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;
SELECT setval('"public"."accounts_id_seq"', 1, true);

-- ----------------------------
-- Sequence structure for ban_seq
-- ----------------------------
DROP SEQUENCE "public"."ban_seq";
CREATE SEQUENCE "public"."ban_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;

-- ----------------------------
-- Sequence structure for channels_id_seq
-- ----------------------------
DROP SEQUENCE "public"."channels_id_seq";
CREATE SEQUENCE "public"."channels_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;

-- ----------------------------
-- Sequence structure for check_event_seq
-- ----------------------------
DROP SEQUENCE "public"."check_event_seq";
CREATE SEQUENCE "public"."check_event_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;
SELECT setval('"public"."check_event_seq"', 1, true);

-- ----------------------------
-- Sequence structure for clan_seq
-- ----------------------------
DROP SEQUENCE "public"."clan_seq";
CREATE SEQUENCE "public"."clan_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 12
 CACHE 1;
SELECT setval('"public"."clan_seq"', 12, true);

-- ----------------------------
-- Sequence structure for clans_id_seq
-- ----------------------------
DROP SEQUENCE "public"."clans_id_seq";
CREATE SEQUENCE "public"."clans_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;
SELECT setval('"public"."clans_id_seq"', 1, true);

-- ----------------------------
-- Sequence structure for contas_seq
-- ----------------------------
DROP SEQUENCE "public"."contas_seq";
CREATE SEQUENCE "public"."contas_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;
SELECT setval('"public"."contas_seq"', 1, true);

-- ----------------------------
-- Sequence structure for gameservers_id_seq
-- ----------------------------
DROP SEQUENCE "public"."gameservers_id_seq";
CREATE SEQUENCE "public"."gameservers_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;

-- ----------------------------
-- Sequence structure for gift_id_seq
-- ----------------------------
DROP SEQUENCE "public"."gift_id_seq";
CREATE SEQUENCE "public"."gift_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;
SELECT setval('"public"."gift_id_seq"', 1, true);

-- ----------------------------
-- Sequence structure for ipsystem_id_seq
-- ----------------------------
DROP SEQUENCE "public"."ipsystem_id_seq";
CREATE SEQUENCE "public"."ipsystem_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;

-- ----------------------------
-- Sequence structure for items_id_seq
-- ----------------------------
DROP SEQUENCE "public"."items_id_seq";
CREATE SEQUENCE "public"."items_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 937
 CACHE 1;
SELECT setval('"public"."items_id_seq"', 937, true);

-- ----------------------------
-- Sequence structure for jogador_amigo_seq
-- ----------------------------
DROP SEQUENCE "public"."jogador_amigo_seq";
CREATE SEQUENCE "public"."jogador_amigo_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;
SELECT setval('"public"."jogador_amigo_seq"', 1, true);

-- ----------------------------
-- Sequence structure for jogador_inventario_seq
-- ----------------------------
DROP SEQUENCE "public"."jogador_inventario_seq";
CREATE SEQUENCE "public"."jogador_inventario_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;
SELECT setval('"public"."jogador_inventario_seq"', 1, true);

-- ----------------------------
-- Sequence structure for jogador_mensagem_seq
-- ----------------------------
DROP SEQUENCE "public"."jogador_mensagem_seq";
CREATE SEQUENCE "public"."jogador_mensagem_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;
SELECT setval('"public"."jogador_mensagem_seq"', 1, true);

-- ----------------------------
-- Sequence structure for loja_seq
-- ----------------------------
DROP SEQUENCE "public"."loja_seq";
CREATE SEQUENCE "public"."loja_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;
SELECT setval('"public"."loja_seq"', 1, true);

-- ----------------------------
-- Sequence structure for message_id_seq
-- ----------------------------
DROP SEQUENCE "public"."message_id_seq";
CREATE SEQUENCE "public"."message_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 51
 CACHE 1;
SELECT setval('"public"."message_id_seq"', 51, true);

-- ----------------------------
-- Sequence structure for player_eqipment_id_seq
-- ----------------------------
DROP SEQUENCE "public"."player_eqipment_id_seq";
CREATE SEQUENCE "public"."player_eqipment_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;
SELECT setval('"public"."player_eqipment_id_seq"', 1, true);

-- ----------------------------
-- Sequence structure for player_friends_player_account_id_seq
-- ----------------------------
DROP SEQUENCE "public"."player_friends_player_account_id_seq";
CREATE SEQUENCE "public"."player_friends_player_account_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;

-- ----------------------------
-- Sequence structure for players_id_seq
-- ----------------------------
DROP SEQUENCE "public"."players_id_seq";
CREATE SEQUENCE "public"."players_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;
SELECT setval('"public"."players_id_seq"', 1, true);

-- ----------------------------
-- Sequence structure for storage_seq
-- ----------------------------
DROP SEQUENCE "public"."storage_seq";
CREATE SEQUENCE "public"."storage_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;
SELECT setval('"public"."storage_seq"', 1, true);

-- ----------------------------
-- Sequence structure for templates_id_seq
-- ----------------------------
DROP SEQUENCE "public"."templates_id_seq";
CREATE SEQUENCE "public"."templates_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;

-- ----------------------------
-- Table structure for accounts
-- ----------------------------
DROP TABLE IF EXISTS "public"."accounts";
CREATE TABLE "public"."accounts" (
"id" int8 DEFAULT nextval('account_id_seq'::regclass) NOT NULL,
"token" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL,
"key" int8 DEFAULT 0 NOT NULL,
"username" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL,
"password" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL,
"email" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL,
"nickname" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL,
"nickcolor" int4 DEFAULT 0 NOT NULL,
"exp" int4 DEFAULT 0 NOT NULL,
"rank" int4 DEFAULT 0 NOT NULL,
"gold" int4 DEFAULT 80000 NOT NULL,
"cash" int4 DEFAULT 40000 NOT NULL,
"online" bool DEFAULT false NOT NULL,
"authority" int4 DEFAULT 0 NOT NULL,
"pccafe" int4 DEFAULT 0 NOT NULL,
"pccafe_date" int4 DEFAULT 0 NOT NULL,
"equipment_primary" int4 DEFAULT 0 NOT NULL,
"equipment_secondary" int4 DEFAULT 601002003 NOT NULL,
"equipment_melee" int4 DEFAULT 702001001 NOT NULL,
"equipment_grenade" int4 DEFAULT 803007001 NOT NULL,
"equipment_special" int4 DEFAULT 904007002 NOT NULL,
"character_red" int4 DEFAULT 1001001005 NOT NULL,
"character_blue" int4 DEFAULT 1001002006 NOT NULL,
"character_helmet" int4 DEFAULT 1102003001 NOT NULL,
"character_beret" int4 DEFAULT 0 NOT NULL,
"character_dino" int4 DEFAULT 1006003041 NOT NULL,
"fights" int4 DEFAULT 0 NOT NULL,
"fights_wins" int4 DEFAULT 0 NOT NULL,
"fights_lost" int4 DEFAULT 0 NOT NULL,
"fights_draw" int4 DEFAULT 0 NOT NULL,
"fights_escapes" int4 DEFAULT 0 NOT NULL,
"kills" int4 DEFAULT 0 NOT NULL,
"deaths" int4 DEFAULT 0 NOT NULL,
"headshots" int4 DEFAULT 0 NOT NULL,
"all_fights" int4 DEFAULT 0 NOT NULL,
"all_kills" int4 DEFAULT 0 NOT NULL,
"broochs" int4 DEFAULT 0 NOT NULL,
"insignias" int4 DEFAULT 0 NOT NULL,
"medals" int4 DEFAULT 0 NOT NULL,
"blueorders" int4 DEFAULT 0 NOT NULL,
"clan_id" int4 DEFAULT 0 NOT NULL,
"clan_authority" int4 DEFAULT 0 NOT NULL,
"clan_date" int4 DEFAULT 0 NOT NULL,
"clan_fights" int4 DEFAULT 0 NOT NULL,
"clan_wins" int4 DEFAULT 0 NOT NULL,
"status" int8 DEFAULT 4294967295::bigint NOT NULL,
"effects" int8 DEFAULT 131072 NOT NULL,
"last_rankup" int4 DEFAULT 1010000 NOT NULL,
"last_login" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL,
"last_ip" varchar COLLATE "default" DEFAULT ''::character varying,
"last_mac" varchar COLLATE "default" DEFAULT ''::character varying,
"last_hwid" varchar COLLATE "default" DEFAULT ''::character varying
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for clan_data
-- ----------------------------
DROP TABLE IF EXISTS "public"."clan_data";
CREATE TABLE "public"."clan_data" (
"clan_id" int4 DEFAULT nextval('clan_seq'::regclass) NOT NULL,
"clan_rank" int4 DEFAULT 0 NOT NULL,
"clan_name" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL,
"owner_id" int8 DEFAULT 0 NOT NULL,
"logo" int8 DEFAULT 0 NOT NULL,
"color" int4 DEFAULT 0 NOT NULL,
"clan_info" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL,
"clan_news" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL,
"create_date" int4 DEFAULT 0 NOT NULL,
"autoridade" int4 DEFAULT 0 NOT NULL,
"limite_rank" int4 DEFAULT 0 NOT NULL,
"limite_idade" int4 DEFAULT 0 NOT NULL,
"limite_idade2" int4 DEFAULT 0 NOT NULL,
"partidas" int4 DEFAULT 0 NOT NULL,
"vitorias" int4 DEFAULT 0 NOT NULL,
"derrotas" int4 DEFAULT 0 NOT NULL,
"pontos" float4 DEFAULT 1000 NOT NULL,
"max_players" int4 DEFAULT 50 NOT NULL,
"clan_exp" int4 DEFAULT 0 NOT NULL,
"best_exp" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL,
"best_participation" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL,
"best_wins" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL,
"best_kills" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL,
"best_headshot" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for clan_invites
-- ----------------------------
DROP TABLE IF EXISTS "public"."clan_invites";
CREATE TABLE "public"."clan_invites" (
"clan_id" int4 DEFAULT 0 NOT NULL,
"player_id" int8 DEFAULT 0 NOT NULL,
"dateinvite" int4 DEFAULT 0 NOT NULL,
"text" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for friends
-- ----------------------------
DROP TABLE IF EXISTS "public"."friends";
CREATE TABLE "public"."friends" (
"owner_id" int8 DEFAULT 0 NOT NULL,
"friend_id" int8 DEFAULT 0 NOT NULL,
"state" int4 DEFAULT 0 NOT NULL,
"removed" bool DEFAULT false NOT NULL
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for info_cupons_flags
-- ----------------------------
DROP TABLE IF EXISTS "public"."info_cupons_flags";
CREATE TABLE "public"."info_cupons_flags" (
"item_id" int4 NOT NULL,
"effect_flag" int8 NOT NULL
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for info_missions
-- ----------------------------
DROP TABLE IF EXISTS "public"."info_missions";
CREATE TABLE "public"."info_missions" (
"mission_id" int4 DEFAULT 0 NOT NULL,
"price" int4 DEFAULT 0 NOT NULL,
"enable" bool DEFAULT false NOT NULL
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for nick_history
-- ----------------------------
DROP TABLE IF EXISTS "public"."nick_history";
CREATE TABLE "public"."nick_history" (
"player_id" int8 DEFAULT 0 NOT NULL,
"from_nick" varchar(255) COLLATE "default" DEFAULT ''::character varying NOT NULL,
"to_nick" varchar(255) COLLATE "default" DEFAULT ''::character varying NOT NULL,
"change_date" int8 DEFAULT 0 NOT NULL,
"motive" varchar(255) COLLATE "default" DEFAULT ''::character varying NOT NULL
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for player_bonus
-- ----------------------------
DROP TABLE IF EXISTS "public"."player_bonus";
CREATE TABLE "public"."player_bonus" (
"player_id" int8 DEFAULT 0 NOT NULL,
"bonuses" int4 DEFAULT 0 NOT NULL,
"sightcolor" int4 DEFAULT 4 NOT NULL,
"freepass" int4 DEFAULT 0 NOT NULL,
"fakerank" int4 DEFAULT 55 NOT NULL,
"fakenick" varchar(255) COLLATE "default" DEFAULT ''::character varying NOT NULL
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for player_configs
-- ----------------------------
DROP TABLE IF EXISTS "public"."player_configs";
CREATE TABLE "public"."player_configs" (
"owner_id" int8 DEFAULT 0 NOT NULL,
"config" int4 DEFAULT 55 NOT NULL,
"sangue" int4 DEFAULT 1 NOT NULL,
"mira" int4 DEFAULT 0 NOT NULL,
"mao" int4 DEFAULT 0 NOT NULL,
"audio1" int4 DEFAULT 100 NOT NULL,
"audio2" int4 DEFAULT 60 NOT NULL,
"audio_enable" int4 DEFAULT 7 NOT NULL,
"sensibilidade" int4 DEFAULT 50 NOT NULL,
"visao" int4 DEFAULT 70 NOT NULL,
"mouse_invertido" int4 DEFAULT 0 NOT NULL,
"msgconvite" int4 DEFAULT 0 NOT NULL,
"chatsussurro" int4 DEFAULT 0 NOT NULL,
"macro" int4 DEFAULT 31 NOT NULL,
"macro_1" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL,
"macro_2" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL,
"macro_3" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL,
"macro_4" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL,
"macro_5" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL,
"keys" bytea DEFAULT '\x'::bytea NOT NULL
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for player_events
-- ----------------------------
DROP TABLE IF EXISTS "public"."player_events";
CREATE TABLE "public"."player_events" (
"player_id" int8 NOT NULL,
"last_visit_event_id" int4 DEFAULT 0 NOT NULL,
"last_visit_sequence1" int4 DEFAULT 0 NOT NULL,
"last_visit_sequence2" int4 DEFAULT 0 NOT NULL,
"next_visit_date" int4 DEFAULT 0 NOT NULL,
"last_xmas_reward_date" int4 DEFAULT 0 NOT NULL,
"last_playtime_date" int4 DEFAULT 0 NOT NULL,
"last_playtime_value" int4 DEFAULT 0 NOT NULL,
"last_playtime_finish" int4 DEFAULT 0 NOT NULL,
"last_login_date" int4 DEFAULT 0 NOT NULL,
"last_quest_date" int4 DEFAULT 0 NOT NULL,
"last_quest_finish" int4 DEFAULT 0 NOT NULL,
"last_daily_cash_date" int4 DEFAULT 0 NOT NULL,
"last_daily_cash_value" int8 DEFAULT 0 NOT NULL
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for player_items
-- ----------------------------
DROP TABLE IF EXISTS "public"."player_items";
CREATE TABLE "public"."player_items" (
"object_id" int8 DEFAULT nextval('items_id_seq'::regclass) NOT NULL,
"owner_id" int8 DEFAULT 0 NOT NULL,
"item_id" int4 DEFAULT 0 NOT NULL,
"item_name" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL,
"count" int4 DEFAULT 0 NOT NULL,
"category" int4 DEFAULT 0 NOT NULL,
"equip" int4 DEFAULT 0 NOT NULL
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for player_messages
-- ----------------------------
DROP TABLE IF EXISTS "public"."player_messages";
CREATE TABLE "public"."player_messages" (
"object_id" int4 DEFAULT nextval('message_id_seq'::regclass) NOT NULL,
"owner_id" int8 DEFAULT 0 NOT NULL,
"sender_id" int8 DEFAULT 0 NOT NULL,
"clan_id" int4 DEFAULT 0 NOT NULL,
"sender_name" varchar(255) COLLATE "default" DEFAULT ''::character varying NOT NULL,
"text" varchar(255) COLLATE "default" DEFAULT ''::character varying NOT NULL,
"type" int4 DEFAULT 0 NOT NULL,
"state" int4 DEFAULT 1 NOT NULL,
"expire" int8 DEFAULT 0 NOT NULL,
"cb" int4 DEFAULT 0 NOT NULL
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for player_missions
-- ----------------------------
DROP TABLE IF EXISTS "public"."player_missions";
CREATE TABLE "public"."player_missions" (
"owner_id" int8 DEFAULT 0 NOT NULL,
"actual_mission" int4 DEFAULT 0 NOT NULL,
"card1" int4 DEFAULT 0 NOT NULL,
"card2" int4 DEFAULT 0 NOT NULL,
"card3" int4 DEFAULT 0 NOT NULL,
"card4" int4 DEFAULT 0 NOT NULL,
"mission_id1" int4 DEFAULT 1 NOT NULL,
"mission_id2" int4 DEFAULT 0 NOT NULL,
"mission_id3" int4 DEFAULT 0 NOT NULL,
"mission1_content" bytea DEFAULT '\x'::bytea NOT NULL,
"mission2_content" bytea DEFAULT '\x'::bytea NOT NULL,
"mission3_content" bytea DEFAULT '\x'::bytea NOT NULL,
"mission4_content" bytea DEFAULT '\x'::bytea NOT NULL
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for player_titles
-- ----------------------------
DROP TABLE IF EXISTS "public"."player_titles";
CREATE TABLE "public"."player_titles" (
"owner_id" int8 DEFAULT 0 NOT NULL,
"titleequiped1" int4 DEFAULT 0 NOT NULL,
"titleequiped2" int4 DEFAULT 0 NOT NULL,
"titleequiped3" int4 DEFAULT 0 NOT NULL,
"titleflags" int8 DEFAULT 0 NOT NULL,
"titleslots" int4 DEFAULT 1 NOT NULL
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for server_block
-- ----------------------------
DROP TABLE IF EXISTS "public"."server_block";
CREATE TABLE "public"."server_block" (
"block_id" int4 NOT NULL,
"address" varchar COLLATE "default",
"mac" varchar COLLATE "default",
"hardware_id" int4,
"bios_id" int4,
"disk_id" int4,
"video_id" int4,
"start_date" date,
"end_date" date,
"reason" varchar COLLATE "default",
"link_video" varchar COLLATE "default",
"link_printscreen" varchar COLLATE "default",
"comment" varchar COLLATE "default"
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for server_info
-- ----------------------------
DROP TABLE IF EXISTS "public"."server_info";
CREATE TABLE "public"."server_info" (
"record_online" int4 DEFAULT 0 NOT NULL
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for shop
-- ----------------------------
DROP TABLE IF EXISTS "public"."shop";
CREATE TABLE "public"."shop" (
"good_id" int4 DEFAULT 0 NOT NULL,
"item_id" int4 DEFAULT 0 NOT NULL,
"item_name" varchar COLLATE "default" DEFAULT ''::character varying NOT NULL,
"price_gold" int4 DEFAULT 0 NOT NULL,
"price_cash" int4 DEFAULT 0 NOT NULL,
"count" int4 DEFAULT 0 NOT NULL,
"buy_type" int4 DEFAULT 0 NOT NULL,
"buy_type2" int4 DEFAULT 0 NOT NULL,
"buy_type3" int4 DEFAULT 0 NOT NULL,
"tag" int4 DEFAULT 0 NOT NULL,
"title" int4 DEFAULT 0 NOT NULL,
"visibility" int4 DEFAULT 0 NOT NULL
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table accounts
-- ----------------------------
ALTER TABLE "public"."accounts" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table clan_data
-- ----------------------------
ALTER TABLE "public"."clan_data" ADD PRIMARY KEY ("clan_id");

-- ----------------------------
-- Primary Key structure for table player_configs
-- ----------------------------
ALTER TABLE "public"."player_configs" ADD PRIMARY KEY ("owner_id");

-- ----------------------------
-- Primary Key structure for table server_block
-- ----------------------------
ALTER TABLE "public"."server_block" ADD PRIMARY KEY ("block_id");
