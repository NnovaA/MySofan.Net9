namespace Ore.Lib.Dto
{
    public class ResponseDto
    {
        public object Result { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = "";
        public string MessageTitle { get; set; } = "";
        public int StatusCode { get; set; }
    }
    public class ResponseDataTable
    {
        public string IdMaster { get; set; }
        public object Master { get; set; }
        public object Data { get; set; }
        public int Draw { get; set; }
        public int DisplayResult { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = "";
        public string MessageTitle { get; set; } = "";
        public object Filter { get; set; }
    }
    public class DtoAttribute
    {
        public long Id { get; set; }
        public long ParentId { get; set; }
        public string Name { get; set; }
    }
}
