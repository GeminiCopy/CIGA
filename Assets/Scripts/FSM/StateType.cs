public enum StateType
{
    Enter,          //进入场景
    Idle,           //原地待机
    ChaseTarget,    //追逐目标，通常用于追逐玩家
    GoToPoint,      //去某个点，通常用与巡逻
    KeepDistance,   //与目标拉扯保持距离，通常用于拉扯玩家
    Attack,         //攻击
    Dead,           //死亡
}
