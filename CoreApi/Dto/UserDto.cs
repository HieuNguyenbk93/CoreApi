namespace CoreApi.Dto
{
    public class UserDto
    {
    }

    public class UserRegisterDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
    }

    public class UserInforDto
    {
        public string UserName { get; set;}
        public string Email { get; set; }
    }
}
