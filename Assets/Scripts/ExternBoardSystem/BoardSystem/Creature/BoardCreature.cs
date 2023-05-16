using ExternBoardSystem.BoardSystem.Position;

namespace ExternBoardSystem.BoardSystem.Creature
{
    public class BoardCreature : BoardElement
    {
        public BoardCreature(CreatureData data) : base(data)
        {
        }

        public CreatureData Data => DataProvider as CreatureData;
    }
}