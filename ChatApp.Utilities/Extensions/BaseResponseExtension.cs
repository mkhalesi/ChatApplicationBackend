
using ChatApp.Dtos.Common;
using ChatApp.Utilities.Constants;

namespace ChatApp.Utilities.Extensions
{
    public static class BaseResponseExtension
    {
        public static BaseResponseDto<T> GenerateSuccessResponse<T>(this BaseResponseDto<T> response, T data)
        {
            response.Success = true;
            response.Data = data;
            response.ErrorCode = null;
            response.ErrorMessage = null;
            return response;
        }

        public static BaseResponseDto<T> GenerateGeneralFailedResponse<T>(this BaseResponseDto<T> response, string errorMessage = null)
        {
            response.Success = false;
            response.Data = default;
            response.ErrorCode = ErrorCodes.InternalServerError;
            response.ErrorMessage = errorMessage;
            return response;
        }

        public static BaseResponseDto<T> GenerateFailedResponse<T>(this BaseResponseDto<T> response, string errorCode, string errorMessage = null)
        {
            response.Success = false;
            response.Data = default;
            response.ErrorCode = errorCode;
            response.ErrorMessage = errorMessage;
            return response;
        }
    }
}
