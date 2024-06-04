namespace ConvenienceMVC.Models.Properties
{
    public class ErrorMessage
    {
        public class MessageData
        {
            public eError MessageNum { get; set; }
            public string? MessageText { get; set; }
        }

        public enum eError
        {
            DataValid = 0,
            NormalUpdate,
            CanNotlUpdate,
            ChumonIdError,
            ChumonDateError,
            ChumonIdRelationError,
            ChumonSuIsNull,
            ChumonSuBadRange,
            ChumonZanIsNull,
            SuErrorBetChumonSuAndZan,
            OtherError
        }

        public static MessageData messageData { get; set; }

        private static readonly ICollection<MessageData> MessageList = new List<MessageData>
        {
            new MessageData { MessageNum=eError.DataValid, MessageText="データチェックＯＫ" },
            new MessageData { MessageNum=eError.NormalUpdate, MessageText="更新しました" },
            new MessageData { MessageNum=eError.CanNotlUpdate, MessageText="更新できませんでした" },
            new MessageData { MessageNum=eError.ChumonIdError, MessageText="注文コード書式エラー" },
            new MessageData { MessageNum=eError.ChumonDateError, MessageText="注文日付エラー" },
            new MessageData { MessageNum=eError.ChumonIdRelationError, MessageText="注文コードアンマッチ" },
            new MessageData { MessageNum=eError.ChumonSuIsNull,MessageText="注文数が設定されていません" },
            new MessageData { MessageNum=eError.ChumonSuBadRange,MessageText="注文数の数値範囲エラーです" },
            new MessageData { MessageNum=eError.ChumonZanIsNull,MessageText="注文残が設定されていません" },
            new MessageData { MessageNum=eError.SuErrorBetChumonSuAndZan,MessageText="注文数と注文残がアンマッチです" },
            new MessageData { MessageNum=eError.OtherError, MessageText="その他エラー" }
        };

        public MessageData SetMessage(eError inError)
        {
            messageData = MessageList.FirstOrDefault(m => m.MessageNum == inError) ?? null;

            return (messageData);
        }
    }
}
