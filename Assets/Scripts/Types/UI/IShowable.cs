using System.Threading.Tasks;

namespace Types.UI {
    public interface IShowable {
        public Task Show(bool immediate = false);
    }
}