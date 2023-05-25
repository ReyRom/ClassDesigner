namespace ClassDesigner.Helping
{
    public interface IHaveMethods: IHaveActions
    {
        Command AddMethodCommand { get; }
    }
}
