namespace Caker.Models
{
    public interface IDtoable<ResponseDto>
    {
        public abstract ResponseDto ToDto();
    }
}
