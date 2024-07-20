using Core.Gameplay;
using Core.Goals;

namespace UI.Panels
{
    public class UICastlesLibraryPanel_CastleItemModel
    {
        private readonly Castle _castle;
        private readonly GameProcessor _gameProcessor;

        public Castle Prefab => _castle;
        public GameProcessor GameProcessor => _gameProcessor;
        public string Id => _castle.Id;

        public UICastlesLibraryPanel_CastleItemModel(Castle castle, GameProcessor gameProcessor)
        {
            _castle = castle;
            _gameProcessor = gameProcessor;
        }
    }
}