using System.Collections.Generic;
using System.Text;
using Vauction.Models.Enums;

namespace Vauction.Models
{
    public interface ICategoryRepository
    {
        object GetCategoriesMapTreeJSON(long event_id, bool demo);
        List<CategoryParentChild> GetCategoriesMapTreeJSON(long event_id, bool demo, string strChek = null);
        EventCategory GetEventCategoryById(long id);
        CategoriesMap GetCategoryMapById(long id);
        List<IdTitle> GetCategoryLeafs(long? category_id);
        List<EventCategoryDetail> GetAllowedCategoriesForTheEvent(long event_id);
        EventCategoryDetail GetEventCategoryDetail(long eventcategory_id);
        CategoryMapDetail GetCategoryMapDetail(long id);
        IEnumerable<IdTitle> GetCategories();
        string GetFullEventCategoryLink(long eventcategory_id);
    }
}
