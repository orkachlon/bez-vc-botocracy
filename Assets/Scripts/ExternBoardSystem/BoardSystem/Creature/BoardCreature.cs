using ExternBoardSystem.BoardSystem.Position;

namespace ExternBoardSystem.BoardSystem.Creature
{
    public class BoardCreature : BoardElement
    {
        public BoardCreature(CreatureElementData elementData) : base(elementData)
        {
        }

        public CreatureElementData ElementData => DataProvider as CreatureElementData;
    }
}