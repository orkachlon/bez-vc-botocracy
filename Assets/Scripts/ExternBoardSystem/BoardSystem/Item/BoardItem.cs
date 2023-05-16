using ExternBoardSystem.BoardSystem.Position;

namespace ExternBoardSystem.BoardSystem.Item
{
    public class BoardItem : BoardElement
    {
        public BoardItem(ItemData data) : base(data)
        {
        }

        public ItemData Data => DataProvider as ItemData;
    }
}