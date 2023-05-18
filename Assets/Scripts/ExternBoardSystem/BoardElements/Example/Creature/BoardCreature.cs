namespace ExternBoardSystem.BoardElements.Example.Creature
{
    public class BoardCreature : BoardElement
    {
        public BoardCreature(SCreatureData data) : base(data)
        {
        }

        public SCreatureData Data => DataProvider as SCreatureData;
    }
}