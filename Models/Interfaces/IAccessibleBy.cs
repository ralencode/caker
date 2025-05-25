namespace Caker.Models.Interfaces
{
    public interface IAccessibleBy
    {
        ICollection<int> AllowedUserIds { get; }
    }
}
