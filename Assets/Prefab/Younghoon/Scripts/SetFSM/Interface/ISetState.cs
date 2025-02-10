namespace BS.Enemy.Set
{
    public interface ISetState
    {
        void Enter();  // 상태 시작 시 호출
        void Update(); // 매 프레임 호출
        void Exit();   // 상태 종료 시 호출
    }
}