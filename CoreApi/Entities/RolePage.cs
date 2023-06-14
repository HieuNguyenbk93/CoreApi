namespace CoreApi.Entities
{
    public class RolePage
    {
        public string RoleId { get; set; }
        public ApplicationRole? Role { get; set; }

        public int PageId { get; set; }
        public PageMenu? Menu { get; set; }
    }
}
