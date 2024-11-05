namespace ShareResource
{
    public interface ICalculate
    {
        int Add(int a, int b);
        int Subtract(int a, int b);
    }

    public class CalculateService : ICalculate
    {
        public int Add(int a, int b) => a + b;
        public int Subtract(int a, int b) => a - b;
    }
}
