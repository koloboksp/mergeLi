using System.Threading;
using System.Threading.Tasks;

namespace Core.Steps.CustomOperations
{
    public class NextBallsShowOperation : Operation
    {
        private readonly bool _show;
        private readonly INextBallsShower _ballsShower;
        
        public NextBallsShowOperation(bool show, INextBallsShower ballsShower)
        {
            _show = show;
            _ballsShower = ballsShower;
        }
        
        protected override async Task<object> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            if(_show)
                _ballsShower.Show();
            else
                _ballsShower.Hide();

            return null;
        }

        public override Operation GetInverseOperation()
        {
            return new NextBallsShowOperation(!_show, _ballsShower);
        }
    }
}