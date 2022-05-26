using ChatApp.Dtos.Models.Paging;

namespace ChatApp.Dtos.Models.Chats
{
    public class FilterPrivateMessagesDTO : BasePaging
    {
        public long ChatId { get; set; }
        public long UserId { get; set; }
        public List<PrivateChatMessageDto>? Messages { get; set; }
        public FilterPrivateMessagesDTO SetPaging(BasePaging paging)
        {
            this.StartPage = paging.StartPage;
            this.PageId = paging.PageId;
            this.PageCount = paging.PageCount;
            this.EndPage = paging.EndPage;
            this.StartPage = paging.StartPage;
            this.TakeEntity = paging.TakeEntity;
            this.SkipEntity = paging.SkipEntity;
            this.ActivePage = paging.ActivePage;
            return this;
        }

        public FilterPrivateMessagesDTO SetChatMessages(List<PrivateChatMessageDto> messages)
        {
            this.Messages = messages;
            return this;
        }
    }
}
