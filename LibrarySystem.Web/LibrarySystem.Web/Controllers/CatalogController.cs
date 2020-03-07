using System.Linq;
using LibraryData;
using LibrarySystem.Web.Models.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Web.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ILibraryAsset _assets;
        private readonly ICheckout _checkout;

        public CatalogController(ILibraryAsset assets, ICheckout checkout)
        {
            _assets = assets;
           _checkout = checkout;
        }

        public IActionResult Index()
        {
            var assetModels = _assets.GetAll();
            var listingResult = assetModels.Select(result => new AssetIndexListingModel
            {
                Id = result.Id,
                ImageUrl = result.ImageUrl,
                AuthorOrDirector = _assets.GetAuthorOrDirector(result.Id),
                DeweyCallNumber = _assets.GetDeweyIndex(result.Id),
                Title = result.Title,
                Type = _assets.GetType(result.Id)
            });

            var model = new AssetIndexModel
            {
                Assets = listingResult
            };

            return View(model);
        }

        // detail action
        public IActionResult Detail(int id)
        {
            var asset = _assets.GetById(id);

            var currentHolds = _checkout.GetCurrentHolds(id)
                .Select(a => new AssetHoldModel
                {
                    HoldPlaced = _checkout.GetCurrentHoldPlaced(a.Id).ToString("d"),
                    PatronName = _checkout.GetCurrentHoldPatronName(a.Id)
                });

            var model = new AssetDetailModel
            {
                AssetId = asset.Id,
                Title = asset.Title,
                Year = asset.Year,
                Cost = asset.Cost,
                Status = asset.Status.Name,
                ImageUrl = asset.ImageUrl,
                AuthorOrDirector = _assets.GetAuthorOrDirector(id),
                CurrentLocation = _assets.GetCurrentLocation(id).Name,
                Dewey = _assets.GetDeweyIndex(id),
                CheckoutHistory = _checkout.GetCheckoutHistory(id),
                ISBN = _assets.GetIsbn(id),
                LatestCheckout = _checkout.GetLatestCheckout(id),
                PatronName = _checkout.GetCurrentHoldPatronName(id),
                CurrentHolds = currentHolds
            };

            return View(model);
        }
    }
}