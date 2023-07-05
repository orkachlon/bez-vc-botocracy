using System.Threading.Tasks;

namespace Types.UI {
    public interface IHideable {

        public Task Hide(bool immediate = false);
    }
}