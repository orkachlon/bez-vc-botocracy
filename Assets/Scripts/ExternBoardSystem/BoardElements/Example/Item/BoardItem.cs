namespace ExternBoardSystem.BoardElements.Example.Item
{
    public class BoardItem : BoardElement
    {
        public BoardItem(SItemData data) : base(data)
        {
        }

        public SItemData ElementData => DataProvider as SItemData;
    }
}