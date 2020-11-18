namespace IBAR.St.Toolkit.Transactions
{
    public enum IbarTransactionLevel
    {
        Serializable = 0,
        RepeatableRead = 1,
        ReadCommitted = 2,
        ReadUncommitted = 3
    }
}
