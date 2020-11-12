namespace Api.CryptoBot.Models.DTO
{
    public class SearchBase
    {
        /// <summary>
        /// The page number this page represents.
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// The size of this page.
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}
