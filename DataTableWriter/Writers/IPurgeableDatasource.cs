namespace DataTableWriter.Writers
{
    public interface IPurgeableDatasource
    {
        void PurgeExpiredData(string table);
    }
}
