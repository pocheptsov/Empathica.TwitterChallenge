namespace Empathica.TwitterChallenge.Db.Domain
{
    /// <summary>
    /// Twitter status object
    /// </summary>
    public class TwitterStatus : IEntity
    {
        public int Id { get; set; }

        public string Text { get; set; }
    }
}