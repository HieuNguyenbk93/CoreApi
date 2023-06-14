namespace CoreApi.Entities
{
    public class PageMenu
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Icon { get; set; }
        public int NumIndex { get; set; }
        public byte Level { get; set; }
        public int ParentId { get; set; }
        public bool IsVisible { get; set; }
        public IList<RolePage> RolePages { get; set; }
    }
}
