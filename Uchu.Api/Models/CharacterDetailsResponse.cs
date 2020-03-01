namespace Uchu.Api.Models
{
    public class CharacterDetailsResponse : BaseResponse
    {
        public long UserId { get; set; }
        
        public long CharacterId { get; set; }
        
        public object Details { get; set; }
    }
}