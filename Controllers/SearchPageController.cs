using optimizely_cms12_azure_ad.Models.Pages;
using optimizely_cms12_azure_ad.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace optimizely_cms12_azure_ad.Controllers;

public class SearchPageController : PageControllerBase<SearchPage>
{
    public ViewResult Index(SearchPage currentPage, string q)
    {
        var model = new SearchContentModel(currentPage)
        {
            Hits = Enumerable.Empty<SearchContentModel.SearchHit>(),
            NumberOfHits = 0,
            SearchServiceDisabled = true,
            SearchedQuery = q
        };

        return View(model);
    }
}
