namespace Canute
{
    public interface IUUIDLabeled
    {
        /// <summary>
        /// UUID of the object
        /// </summary>
        UUID UUID { get; set; }
    }
    public static class UUIDLabeled
    {
        public static UUID GenerateUUID(this IUUIDLabeled item)
        {
            if (item.UUID != UUID.Empty)
            {
                return item.UUID;
            }
            item.UUID = UUID.NewUUID();
            return item.UUID;
        }
        public static UUID NewUUID(this IUUIDLabeled item)
        {
            item.UUID = UUID.NewUUID();
            return item.UUID;
        }

    }
}