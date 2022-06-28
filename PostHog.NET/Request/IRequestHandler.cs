using System.Threading.Tasks;
using PostHog.Model;

namespace PostHog.Request
{
    internal interface IRequestHandler
    {
        Task MakeRequest(Batch batch);
    }
}