namespace DogInfoTracker {
    public class ActivityInformation {
        //Id, StartTime, EndTime, Duration
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public ActivityType ActivityType { get; set; }
        public int Cost { get; set; }
        public string? Description { get; set; }
        public float Weight { get; set; }
    }
}
