namespace ChatApp.Dtos.Common
{
    public class BaseResponseDto<T>
    {
        public T Data { get; set; }

        public bool Success { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
