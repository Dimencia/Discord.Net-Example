using System.Threading.Tasks;

namespace InactiviteRoleRemover
{
    class Program
    {
        public static Task Main(string[] args)
            => Startup.RunAsync(args);
    }
}
