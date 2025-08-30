namespace Rox.GameExpansionFeatures
{
    public partial class CSGO
    {
        /// <summary>
        /// CSGO / CS2 配置命令类
        /// </summary>
        public class Config
        {
            /// <summary>
            /// 管理游戏规则和多人对战相关的设置，通常只有服务器管理员可以修改。
            /// </summary>
            /// <remarks>在控制台中以 <see langword="mp_"/> 为开头</remarks>
            public class MultiPlayer
            {
                /// <summary>
                /// 启用或禁用仅爆头伤害。
                /// </summary>
                /// <param name="Enable"> 启用或禁用仅爆头伤害</param>
                /// <returns> mp_damage_headshot_only 0 / 1</returns>
                public static string EnableOnlyHeadShotsDamage(bool Enable) => $"mp_damage_headshot_only {(Enable ? "1" : "0")}";
                /// <summary>
                /// 是否允许在游戏中丢弃刀具。
                /// </summary>
                /// <param name="Enable"> 是否允许玩家丢弃刀具</param>
                /// <returns> mp_drop_knife_enable 0 / 1</returns>
                public static string AllowDropKnife(bool Enable) => $"mp_drop_knife_enable {(Enable ? "1" : "0")}";
                /// <summary>
                /// 是否允许使用 Zeus 电击枪。
                /// </summary>
                /// <param name="Enable"> 是否允许使用 Zeus 电击枪</param>
                /// <returns> mp_weapons_allow_zeus 0 / 1</returns>
                public static string AllowUseZeus(bool Enable) => $"mp_weapons_allow_zeus {(Enable ? "1" : "0")}";
                /// <summary>
                /// 设置回合开始时自带的护甲
                /// </summary>
                /// <param name="a"> 0 = 无护甲, 1 = 仅护甲 , 2 = 头盔和护甲</param>
                /// <returns> mp_free_armor 0 / 1 / 2</returns>
                public static string SetStartWithArmor(int a) => $"mp_free_armor {(a < 0 ? "2" : (a > 2 ? "2" : a.ToString()))}";
                /// <summary>
                /// 是否允许丢弃投掷物
                /// </summary>
                /// <param name="Enable"> 是否允许丢弃投掷物</param>
                /// <returns> mp_drop_grenade_enable 0 / 1</returns>
                public static string AllowDropGrenade(bool Enable) => $"mp_drop_grenade_enable {(Enable ? "1" : "0")}";
                /// <summary>
                /// 是否允许死亡后掉落拆弹工具
                /// </summary>
                /// <param name="Enable"> 是否允许死亡后掉落拆弹工具</param>
                /// <returns> mp_death_drop_defuser 0 / 1</returns>
                public static string AllowDropDefuseKitWithDeath(bool Enable) => $"mp_death_drop_defuser {(Enable ? "1" : "0")}";
                /// <summary>
                /// 设置开始时是否分配拆弹工具
                /// </summary>
                /// <param name="a"> 0 = 不分配, 1 = 随机分配, 2 = 全部分配</param>
                /// <returns> mp_defuser_allocation 0 / 1 / 2</returns>
                public static string SetStartWithDefuseKit(int a) => $"mp_defuser_allocation {(a < 0 ? "2" : (a > 2 ? "2" : a.ToString()))}";
                /// <summary>
                /// 设置死亡竞赛的回合时间 (最大不超过 60 分钟)
                /// </summary>
                /// <param name="minutes"> 回合时间（分钟）最大不超过 60 分钟</param>
                /// <returns> mp_roundtime 0 - 60</returns>
                public static string SetRoundTime_Deathmatch(int minutes) => $"mp_roundtime {(minutes < 0 ? "10" : (minutes >= 60 ? "60" : minutes.ToString()))}";
                /// <summary>
                /// 设置休闲模式 / 竞技模式的回合时间 (最大不超过 60 分钟)
                /// </summary>
                /// <param name="minutes"></param>
                /// <returns></returns>
                public static string SetRoundTime_Competitive_Casual(int minutes) => $"mp_roundtime_defuse {(minutes < 0 ? "1" : (minutes > 60 ? "60" : minutes.ToString()))}";
                /// <summary>
                /// 设置休闲模式 / 竞技模式的最大回合数 (最大不超过 60, 0 为无限回合)
                /// </summary>
                /// <param name="rounds"> 最大回合数 (最大不超过 60, 0 为无限回合)</param>
                /// <returns> mp_maxrounds 0 - 60</returns>
                public static string SetMaxRound(int rounds) => $"mp_maxrounds {(rounds < 0 ? "24" : (rounds > 60 ? "60" : rounds.ToString()))}";
                /// <summary>
                /// 设置休闲模式 / 竞技模式的回合开始的冻结时间 (最大不超过 60 秒)
                /// </summary>
                /// <param name="seconds"> 冻结时间（秒）最大不超过 60 秒</param>
                /// <returns> mp_freezetime 0 - 60</returns>
                public static string SetFreezeTime(int seconds) => $"mp_freezetime {(seconds < 0 ? "6" : (seconds > 60 ? "60" : seconds.ToString()))}";
                /// <summary>
                /// 设置购买时间 (9999 秒为无限时间, 0 为禁用购买时间)
                /// </summary>
                /// <param name="seconds"> 9999 秒为无限时间, 0 为禁用购买时间</param>
                /// <returns> mp_buytime 0 - 9999</returns>
                public static string SetBuyTime(int seconds) => $"mp_buytime {(seconds < 0 ? "0" : (seconds > 9999 ? "9999" : seconds.ToString()))}";
                /// <summary>
                /// 设置允许在任何地方购买武器和装备
                /// </summary>
                /// <param name="Enable"> 0 为仅在出生点购买, 1 为允许在任何地方购买</param>
                /// <returns> mp_buy_anywhere 0 / 1</returns>
                public static string AllowBuyAnywhere(bool Enable) => $"mp_buy_anywhere {(Enable ? "1" : "0")}";
                /// <summary>
                /// 设置是否允许随机复活
                /// </summary>
                /// <param name="Enable"> 是否允许随机复活</param>
                /// <returns> mp_randomspawn 0 / 1</returns>
                public static string AllowRandomSpawns(bool Enable) => $"mp_randomspawn {(Enable ? "1" : "0")}";
                /// <summary>
                /// 设置重生后的无敌时间 (最大不超过 20 秒)
                /// </summary>
                /// <param name="seconds"> 无敌时间（秒）最大不超过 20 秒</param>
                /// <returns> mp_respawn_immunitytime 0 - 20</returns>
                public static string SetImmunityTime_Deathmatch(int seconds) => $"mp_respawn_immunitytime {(seconds < 0 ? "0" : (seconds > 20 ? "20" : seconds.ToString()))}";
                /// <summary>
                /// 启用或禁用友军伤害
                /// </summary>
                /// <param name="Enable"> 启用或禁用友军伤害</param>
                /// <returns> mp_friendlyfire 0 / 1</returns>
                public static string EnableFriendlyFire(bool Enable) => $"mp_friendlyfire {(Enable ? "1" : "0")}";
                /// <summary>
                /// 启用或禁用自动踢出玩家, 通常用于挂机 (AFK) 和 太多的团队伤害以及杀戮队友行为
                /// </summary>
                /// <param name="Enable"> 启用或禁用自动踢出玩家</param>
                /// <returns> mp_autokick 0 / 1</returns>
                public static string EnableAutoKick(bool Enable) => $"mp_autokick {(Enable ? "1" : "0")}";
                /// <summary>
                /// 设置C4炸弹的计时器时间 (最大不超过 <see cref="int.MaxValue"/> = 2147483647 秒)
                /// </summary>
                /// <param name="seconds"> 计时器时间（秒）最大不超过 <see cref="int.MaxValue"/> = 2147483647 秒</param>
                /// <returns> mp_c4timer 0 - 2147483647</returns>
                public static string SetC4Time(int seconds) => $"mp_c4timer {(seconds < 0 ? "40" : (seconds > int.MaxValue ? int.MaxValue.ToString() : seconds.ToString()))}";
                /// <summary>
                /// 启用或禁用在中场休息时切换队伍
                /// </summary>
                /// <param name="Enable"> 启用或禁用在中场休息时切换队伍</param>
                /// <returns> mp_halftime 0 / 1</returns>
                public static string EnableHalfTime(bool Enable) => $"mp_halftime {(Enable ? "1" : "0")}";
                /// <summary>
                /// 设置中场休息的持续时间 (最大不超过 60 秒)
                /// </summary>
                /// <param name="seconds"> 中场休息的持续时间（秒）最大不超过 60 秒</param>
                /// <returns> mp_halftime_duration 0 - 60</returns>
                public static string SetHalfFreezeTime(int seconds) => $"mp_halftime_duration {(seconds < 0 ? "15" : (seconds > 300 ? "300" : seconds.ToString()))}";
                /// <summary>
                /// 设置服务器的最大金钱限制 (最大不超过 65535)
                /// </summary>
                /// <param name="money"> 最大金钱限制（单位为游戏内货币）</param>
                /// <returns> mp_maxmoney 0 - 65535</returns>
                public static string SetMaxMoney(int money) => $"mp_maxmoney {(money < 0 ? "16000" : (money >= int.MaxValue ? int.MaxValue.ToString() : money.ToString()))}";
                /// <summary>
                /// 设置每回合的初始金钱 (最大不超过 65535, 小于 0 则默认为 800)
                /// </summary>
                /// <param name="money"> 初始金钱（单位为游戏内货币）</param>
                /// <returns> mp_startmoney 0 - 65535</returns>
                public static string SetStartMoney(int money) => $"mp_startmoney {(money < 0 ? "800" : (money >= int.MaxValue ? int.MaxValue.ToString() : money.ToString()))}";
                /// <summary>
                /// 设置x秒后重启游戏,如果小于0则默认为1秒, 大于60则默认为60秒
                /// </summary>
                /// <param name="seconds"> 重启游戏的秒数</param>
                /// <returns> mp_restartgame 1 - 60</returns>
                public static string SetRestartGameSeconds(int seconds) => $"mp_restartgame {(seconds < 0 ? "1" : (seconds > 60 ? "60" : seconds.ToString()))}";
                /// <summary>
                /// 选择直接结束热身时间
                /// </summary>
                /// <returns> mp_warmup_end</returns>
                public static string EndWarmup() => "mp_warmup_end";
                /// <summary>
                /// 设置热身时间, 如果小于0则默认为0, 大于600则默认为600秒
                /// </summary>
                /// <param name="seconds"> 热身时间（秒）</param>
                /// <returns> mp_warmuptime 0 - 600</returns>
                public static string SetWarmupTime(int seconds) => $"mp_warmuptime {(seconds < 0 ? "0" : (seconds > int.MaxValue ? int.MaxValue.ToString() : seconds.ToString()))}";
                /// <summary>
                /// 设置是否启用无限热身时间
                /// </summary>
                /// <param name="Enable"> 是否启用无限热身时间</param>
                /// <returns> mp_warmup_pausetimer 0 / 1</returns>
                public static string SetInfiniteWarmupTime(bool Enable) => $"mp_warmup_pausetimer {(Enable ? "1" : "0")}";
                /// <summary>
                /// 启用或禁用自动平衡队伍人数
                /// </summary>
                /// <param name="Enable"> 启用或禁用自动平衡队伍人数</param>
                /// <returns> mp_autoteambalance 0 / 1</returns>
                public static string EnableAutoTeamBalance(bool Enable) => $"mp_autoteambalance {(Enable ? "1" : "0")}";
                /// <summary>
                /// 设置每个队伍的最大玩家数量 (最大不超过 20, 小于 0 则为不限制)
                /// </summary>
                /// <param name="number"> 每个队伍的最大玩家数量</param>
                /// <returns> mp_limitteams 0 - 20</returns>
                public static string SetLimitTeamNumber(int number) => $"mp_limitteams {(number < 0 ? "0" : (number > 20 ? "20" : number.ToString()))}";
            }
            /// <summary>
            /// 控制服务器端的核心参数，影响所有玩家的游戏体验，通常只有服务器管理员可以修改。
            /// </summary>
            /// <remarks>在控制台中以 <see langword="sv_"/> 为开头</remarks>
            public class Server
            {
                /// <summary>
                /// 启用或禁用服务器上的作弊模式。
                /// </summary>
                /// <param name="Enable"> 启用或禁用作弊模式</param>
                /// <returns> sv_cheats 0 / 1</returns>
                public static string EnableCheats(bool Enable) => $"sv_cheats {(Enable ? "1" : "0")}";
                /// <summary>
                /// 允许购买的团队, 0 = 所有团队, 1 = 仅反恐精英能购买, 2 = 仅恐怖分子能购买, 3 = 均不能购买
                /// </summary>
                /// <param name="who"></param>
                /// <returns></returns>
                public static string SetBuyStates(int who) => $"sv_buy_status_override {(who < 0 ? "0" : (who > 3 ? "0" : who.ToString()))}";
                /// <summary>
                /// 启用或禁用自动跳跃功能。
                /// </summary>
                /// <param name="Enable"> 是否启用自动跳跃功能</param>
                /// <returns>sv_autobunnyhopping 0 / 1</returns>
                public static string EnableAutoBunnyHopping(bool Enable) => $"sv_autobunnyhopping {(Enable ? "1" : "0")}";
                /// <summary>
                /// 启用或禁用连续跳跃限速
                /// </summary>
                /// <param name="Enable"> 是否启用连续跳跃限速</param>
                /// <returns> sv_enablebunnyhopping 0 / 1</returns>
                public static string EnableBunnyHopping(bool Enable) => $"sv_enablebunnyhopping {(Enable ? "1" : "0")}";
                /// <summary>
                /// 设置空中加速的最大速度限制, 默认值为 12, 最大值为 1000
                /// </summary>
                /// <param name="speed"> 最大速度限制 (小于 0 则默认为 12, 大于 1000 则默认为 1000)</param>
                /// <returns> sv_airaccelerate 12 - 1000</returns>
                public static string SetAirLimitMaxSpeed(int speed) => $"sv_airaccelerate {(speed < 0 ? "12" : (speed > 1000 ? "1000" : speed.ToString()))}";
                /// <summary>
                /// 启用或禁用自动恢复生命值功能。
                /// </summary>
                /// <param name="Enable"> 是否启用自动恢复生命值功能</param>
                /// <returns> sv_regeneration_force_on 0 / 1</returns>
                public static string EnableHealthAutoRecovery(bool Enable) => $"sv_regeneration_force_on {(Enable ? "1" : "0")}";
                /// <summary>
                /// 是否显示子弹落点
                /// </summary>
                /// <param name="level"> 0 = 不显示, 1 = 显示客户端 (红色) 和服务器 (蓝色) 方块, 2 = 仅显示客户端 (红色) 子弹撞击位置, 3 = 仅显示服务器 (蓝色) 子弹撞击位置</param>
                /// <returns> sv_showimpacts 0 / 1 / 2</returns>
                public static string EnableShowImpacts(int level) => $"sv_showimpacts {(level < 0 ? "0" : (level > 2 ? "2" : level.ToString()))}";
                /// <summary>
                /// 设置显示子弹落点的持续时间 (最大不超过 60 秒, 小于 0 则默认为 4 秒)
                /// </summary>
                /// <param name="seconds"> 持续时间（秒）最大不超过 60 秒</param>
                /// <returns> sv_showimpacts_time 0 - 60</returns>
                public static string SetShowImpactsTime(int seconds) => $"sv_showimpacts_time {(seconds < 0 ? "4" : (seconds > 60 ? "60" : seconds.ToString()))}";
                /// <summary>
                /// 显示额外的子弹穿透信息
                /// </summary>
                /// <param name="Enable"> 是否启用显示额外的子弹穿透信息</param>
                /// <returns> sv_showimpacts_penetration 0 / 1</returns>
                public static string EnableShowImpactsExtraInfo(bool Enable) => $"sv_showimpacts_penetration {(Enable ? "1" : "0")}";
                /// <summary>
                /// 启用或禁用无限弹药, 0 = 禁用, 1 = 当前弹夹无限 / 投掷物无限, 2 = 备用弹夹无限 / 投掷物无限
                /// </summary>
                /// <param name="level"> 0 = 禁用, 1 = 当前弹夹无限 / 投掷物无限, 2 = 备用弹夹无限 / 投掷物无限</param>
                /// <returns> sv_infinite_ammo 0 / 1 / 2</returns>
                public static string EnableInfiniteAmmo(int level) => $"sv_infinite_ammo {(level < 0 ? "0" : (level >= 2 ? "2" : level.ToString()))}";
                /// <summary>
                /// 启用或禁用投掷物轨迹显示
                /// </summary>
                /// <param name="Enable"> 是否启用投掷物轨迹显示</param>
                /// <returns> sv_grenade_trajectory 0 / 1</returns>
                public static string EnableShowGredeTrajectory(bool Enable) => $"sv_grenade_trajectory {(Enable ? "1" : "0")}";
                /// <summary>
                /// 设置投掷物轨迹的粗细
                /// </summary>
                /// <param name="Thickness"> 粗细 (小于 0 则默认为 0.2, 大于 1000 则默认为 1000)</param>
                /// <returns> sv_grenade_trajectory_thickness 0.2 - 1000</returns>
                public static string SetShowGrenadeTrajectorysThickness(decimal Thickness) => $"sv_grenade_trajectory_thickness  {(Thickness < 0 ? "0.2" : (Thickness >= decimal.MaxValue ? "1000" : Thickness.ToString()))}";
                /// <summary>
                /// 设置投掷物轨迹的持续时间 (最大不超过 60 秒, 小于 0 则默认为 20 秒)
                /// </summary>
                /// <param name="seconds"> 持续时间（秒）最大不超过 60 秒</param>
                /// <returns> sv_grenade_trajectory_time 0 - 60</returns>
                public static string SetShowGrenadeTrajectorysTime(int seconds) => $"sv_grenade_trajectory_time {(seconds < 0 ? "20" : (seconds > 60 ? "60" : seconds.ToString()))}";
                /// <summary>
                /// 重新抛出上次投掷的投掷物
                /// </summary>
                /// <returns> sv_rethrow_last_grenade</returns>
                public static string RethrowLastGrenade() => "sv_rethrow_last_grenade ";
                /// <summary>
                /// 启用或禁用手雷无线电提示
                /// </summary>
                /// <param name="Enable"> 是否启用手雷无线电提示</param>
                /// <returns> sv_ignoregrenaderadio 0 / 1</returns>
                public static string EnableGrenadeRadio(bool Enable) => $"sv_ignoregrenaderadio {(Enable ? "1" : "0")}";
                /// <summary>
                /// 设置行走加速度 (最大不超过 20, 小于 0 则默认为 5.6)
                /// </summary>
                /// <param name="acceleration"> 行走加速度 (最大不超过 20, 小于 0 则默认为 5.6)</param>
                /// <returns> sv_accelerate 0 - 20</returns>
                public static string SetWalkingAcceleration(decimal acceleration) => $"sv_accelerate {(acceleration < 0 ? "5.6" : (acceleration > decimal.MaxValue ? "20" : acceleration.ToString()))}";
                /// <summary>
                /// 设置地面摩擦力 (最大不超过 20, 小于 0 则默认为 5.2)
                /// </summary>
                /// <param name="friction"> 地面摩擦力 (最大不超过 20, 小于 0 则默认为 5.2)</param>
                /// <returns> sv_friction 0 - 20</returns>
                public static string SetGroundFriction(decimal friction) => $"sv_friction {(friction < 0 ? "5.2" : (friction > decimal.MaxValue ? "20" : friction.ToString()))}";
                /// <summary>
                /// 设置玩家的最大速度 (最大不超过 <see cref="int.MaxValue"/> = 2147483647, 小于 0 则默认为 320, 大于 1000 则默认为 1000)
                /// </summary>
                /// <param name="speed"> 最大速度 (最大不超过 <see cref="int.MaxValue"/> = 2147483647, 小于 0 则默认为 320, 大于 1000 则默认为 1000)</param>
                /// <returns> sv_maxspeed 0 - 1000</returns>
                public static string SetMaxSpeed(int speed) => $"sv_maxspeed {(speed < 0 ? "320" : (speed > int.MaxValue ? "1000" : speed.ToString()))}";
                /// <summary>
                /// 设置停止速度，值越大加速越慢，急停越快
                /// </summary>
                /// <param name="speed"> 停止速度 (小于 0 则默认为 80, 大于 <see cref="int.MaxValue"/> = 2147483647 则默认为 1000)</param>
                /// <returns> sv_stopspeed 0 - 1000</returns>
                public static string SetStopSpeed(int speed) => $"sv_stopspeed {(speed < 0 ? "80" : (speed > int.MaxValue ? "1000" : speed.ToString()))}";
            }
        }
    }
}
