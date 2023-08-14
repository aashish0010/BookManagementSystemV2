namespace BookManagementSystem.Domain.Entities
{
    public class Common
    {
        public Level Status { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
    }
    public enum Level
    {
        Success = 000,
        Failed = 001
    }
}
