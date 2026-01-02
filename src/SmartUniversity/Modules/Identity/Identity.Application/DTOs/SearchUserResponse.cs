namespace SmartUniversity.Modules.Identity.Application.DTO
{
    public class SearchUserResponse
    {
        public List<UserResponse> Data { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class SearchUserResponseWrapper {
      public SearchUserResponse Data {get; set;}
    }
}
