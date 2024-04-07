namespace Types.StoryPoint {
    
    public interface ISPProvider {
        int Count { get; }
        
        IStoryPointData Next();
        void AddOutcome(int outcomeID);
        void RemoveOutcome(int outcomeID);
    }
}
