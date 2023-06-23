namespace CoreApi.Dto.AuthDto
{
    public class TokenDto
    {
        public double Expires_in { get; set; }

        public string Access_token { get; set; }

        public string Refresh_token { get; set; }
    }
}
